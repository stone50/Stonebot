namespace Stonebot {
    using Models.EventSubMessages;
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Threading;
    using System.Threading.Tasks;
    using Twitch;

    internal static class WebSocketClient {
        public static event EventHandler ClosedUnexpectedly = delegate { };

        public static string? Id { get; private set; }

        public static Task<bool> TryConnectAsync(CancellationToken cancellationToken) {
            var url = Utils.GetUrl("wss://eventsub.wss.twitch.tv/ws", new() { { "keepalive_timeout_seconds", Config.SocketKeepaliveTimeoutSeconds.ToString() } });
            return ConnectToAsync(url, cancellationToken);
        }

        public static Task CloseAsync(CancellationToken cancellationToken) => CloseAsync(WebSocketCloseStatus.NormalClosure, null, false, cancellationToken);

        private class CloseException(string? reconnectUrl = null) : Exception {
            public readonly string? ReconnectUrl = reconnectUrl;
        }

        private static ClientWebSocket? socket;
        private static Task? listenTask;
        private static CancellationTokenSource? cancellationTokenSource;

        private static async Task<bool> ConnectToAsync(string url, CancellationToken cancellationToken) {
            socket = new();
            Id = await ConnectSocketToAsync(socket, url, cancellationToken).ConfigureAwait(false);
            if (Id is null) {
                socket = null;
                return false;
            }

            cancellationTokenSource = new();
            listenTask = ListenAction(cancellationTokenSource.Token);
            return true;
        }

        private static async Task CloseAsync(WebSocketCloseStatus status, string? statusDescription, bool isUnexpectedClose, CancellationToken cancellationToken) {
            if (cancellationTokenSource is not null) {
                await cancellationTokenSource.CancelAsync().ConfigureAwait(false);
            }

            listenTask = null;
            cancellationTokenSource = null;
            if (socket is not null) {
                if (socket.State == WebSocketState.Open) {
                    await socket.CloseAsync(status, statusDescription, cancellationToken).ConfigureAwait(false);
                }

                socket = null;
            }

            Id = null;

            if (isUnexpectedClose) {
                ClosedUnexpectedly.Invoke(null, new());
            }
        }

        private static async Task<string?> ConnectSocketToAsync(ClientWebSocket socket, string url, CancellationToken cancellationToken) {
            var socketUri = new Uri(url);
            await socket.ConnectAsync(socketUri, cancellationToken).ConfigureAwait(false);
            var request = await GetRequestAsync(socket, cancellationToken).ConfigureAwait(false);
            if (request is null) {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                return null;
            }

            var welcomeMessage = JsonSerializer.Deserialize(request, JsonContext.Default.EventSubWelcomeMessage);
            return welcomeMessage.Payload.Session.Id;
        }

        private static async Task ListenAction(CancellationToken cancellationToken) {
            if (socket is null) {
                return;
            }

            while (!cancellationToken.IsCancellationRequested) {
                try {
                    var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Config.SocketKeepaliveTimeoutSeconds));
                    var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
                    var request = await GetRequestAsync(socket, linkedCancellationTokenSource.Token).ConfigureAwait(false);
                    if (request is null) {
                        FireClose(WebSocketCloseStatus.NormalClosure, "Close message received.", true);
                        return;
                    }

                    if (TryParseRequest(request, JsonContext.Default.EventSubKeepaliveMessage, out var keepaliveMessage) && keepaliveMessage.Metadata.MessageType == "session_keepalive") {
                        continue;
                    }

                    if (TryParseRequest(request, JsonContext.Default.EventSubNotificationMessage, out var notificationMessage) && notificationMessage.Metadata.MessageType == "notification") {
                        ChatMessageHandler.HandleChatMessage(notificationMessage.Payload.Event);
                        continue;
                    }

                    if (TryParseRequest(request, JsonContext.Default.EventSubReconnectMessage, out var reconnectMessage) && reconnectMessage.Metadata.MessageType == "session_reconnect") {
                        HandleReconnectAsync(reconnectMessage, cancellationToken);
                        continue;
                    }

                    if (TryParseRequest(request, JsonContext.Default.EventSubRevocationMessage, out var revocationMessage) && revocationMessage.Metadata.MessageType == "revocation") {
                        await HandleRevocationAsync(revocationMessage, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    FireClose(WebSocketCloseStatus.InvalidMessageType, null, true);
                    return;
                } catch (OperationCanceledException) {
                    FireClose(WebSocketCloseStatus.NormalClosure, "Operation cancelled.", false);
                    return;
                } catch (Exception e) {
                    Logger.Error(e);
                    FireClose(WebSocketCloseStatus.InternalServerError, e.Message, true);
                    return;
                }
            }
        }

        private static async Task<string?> GetRequestAsync(ClientWebSocket socket, CancellationToken cancellationToken) {
            var buffer = new byte[65536];
            var result = await socket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
            return result.MessageType == WebSocketMessageType.Close ? null : Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        private static async void FireClose(WebSocketCloseStatus status, string? statusDescription, bool isUnexpectedClose) {
            var closeCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.WebSocketClientCloseOnCancelTimeoutSeconds));
            try {
                await CloseAsync(status, statusDescription, isUnexpectedClose, closeCancellationTokenSource.Token);
            } catch (Exception e) {
                Logger.Error(e);
            }
        }

        private static bool TryParseRequest<T>(string request, JsonTypeInfo<T> jsonTypeInfo, out T requestData) where T : struct {
            try {
                requestData = JsonSerializer.Deserialize(request, jsonTypeInfo);
            } catch {
                requestData = default;
                return false;
            }

            return true;
        }

        private static async void HandleReconnectAsync(EventSubReconnectMessage message, CancellationToken cancellationToken) {
            var newSocket = new ClientWebSocket();
            var newId = await ConnectSocketToAsync(newSocket, message.Payload.Session.ReconnectUrl!, cancellationToken).ConfigureAwait(false);
            await CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnect message received.", false, cancellationToken).ConfigureAwait(false);
            socket = newSocket;
            Id = newId;
            cancellationTokenSource = new();
            listenTask = ListenAction(cancellationTokenSource.Token);
        }

        private static async Task HandleRevocationAsync(EventSubRevocationMessage message, CancellationToken cancellationToken) {
            var subscription = message.Payload.Subscription;
            await EventSub.DeleteEventSubAsync(subscription.Id, cancellationToken).ConfigureAwait(false);
            Logger.Warn("Event subscription revoked.", subscription.Type);
        }
    }
}
