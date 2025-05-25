namespace Stonebot {
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

        public static void Connect(CancellationToken cancellationToken) {
            var url = Utils.GetUrl("wss://eventsub.wss.twitch.tv/ws", new() { { "keepalive_timeout_seconds", Config.SocketKeepaliveTimeoutSeconds.ToString() } });
            ConnectTo(url, cancellationToken);
            EventSub.SubscribeToChannelChatMessage(cancellationToken);
        }

        public static void Close(CancellationToken cancellationToken) {
            Close(WebSocketCloseStatus.NormalClosure, null, false, cancellationToken);
            if (Cache.ChatterAuthorizationData is not null) {
                EventSub.DeleteEventSubs(cancellationToken);
            }
        }

        private static ClientWebSocket? socket;
        private static Task? listenTask;
        private static CancellationTokenSource? cancellationTokenSource;

        private static void ConnectTo(string url, CancellationToken cancellationToken) {
            socket = new();
            Id = ConnectSocketTo(socket, url, cancellationToken);
            cancellationTokenSource = new();
            listenTask = Task.Run(() => ListenAction(cancellationTokenSource.Token), CancellationToken.None);

        }

        private static void Close(WebSocketCloseStatus status, string? statusDescription, bool isUnexpectedClose, CancellationToken cancellationToken) {
            cancellationTokenSource?.Cancel();
            if (listenTask is not null) {
                Utils.Sync(listenTask);
            }

            listenTask = null;
            cancellationTokenSource = null;
            if (socket is not null) {
                if (socket.State == WebSocketState.Open) {
                    var closeTask = socket.CloseAsync(status, statusDescription, cancellationToken);
                    Utils.Sync(closeTask);
                }

                socket = null;
            }

            Id = null;
            if (isUnexpectedClose) {
                EventSub.DeleteEventSubs(cancellationToken);
                ClosedUnexpectedly.Invoke(null, new());
            }
        }

        private static string ConnectSocketTo(ClientWebSocket socket, string url, CancellationToken cancellationToken) {
            var socketUri = new Uri(url);
            var connectTask = socket.ConnectAsync(socketUri, cancellationToken);
            Utils.Sync(connectTask);
            try {
                var request = GetRequest(socket, cancellationToken);
                var welcomeMessage = JsonSerializer.Deserialize(request!, JsonContext.Default.EventSubWelcomeMessage);
                return welcomeMessage.Payload.Session.Id;
            } catch (Exception e) {
                var closeTask = socket.CloseAsync(WebSocketCloseStatus.InternalServerError, e.Message, cancellationToken);
                Utils.Sync(closeTask);
                throw;
            }
        }

        private static void ListenAction(CancellationToken cancellationToken) {
            if (socket is null) {
                return;
            }

            while (!cancellationToken.IsCancellationRequested) {
                try {
                    var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Config.SocketKeepaliveTimeoutSeconds));
                    var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
                    var request = GetRequest(socket, linkedCancellationTokenSource.Token);
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
                        FireReconnect(reconnectMessage.Payload.Session.ReconnectUrl, cancellationToken);
                        continue;
                    }

                    if (TryParseRequest(request, JsonContext.Default.EventSubRevocationMessage, out var revocationMessage) && revocationMessage.Metadata.MessageType == "revocation") {
                        var subscription = revocationMessage.Payload.Subscription;
                        EventSub.DeleteEventSub(subscription.Id, cancellationToken);
                        Logger.Warn("Event subscription revoked.", subscription.Status);
                        continue;
                    }

                    FireClose(WebSocketCloseStatus.InvalidMessageType, null, true);
                    return;
                } catch (OperationCanceledException) {
                    FireClose(WebSocketCloseStatus.NormalClosure, "Operation cancelled.", false);
                    return;
                } catch (Exception e) {
                    try {
                        Logger.Error(e);
                    } finally {
                        FireClose(WebSocketCloseStatus.InternalServerError, e.Message, true);
                    }

                    return;
                }
            }
        }

        private static string? GetRequest(ClientWebSocket socket, CancellationToken cancellationToken) {
            var buffer = new byte[65536];
            var receiveTask = socket.ReceiveAsync(buffer, cancellationToken);
            var result = Utils.Sync(receiveTask);
            return result.MessageType == WebSocketMessageType.Close ? null : Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        private static void FireClose(WebSocketCloseStatus status, string? statusDescription, bool isUnexpectedClose) {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.WebSocketClientFireCloseTimeoutSeconds)).Token;
            _ = Task.Run(() => Utils.TryElseError(() => Close(status, statusDescription, isUnexpectedClose, cancellationToken)), cancellationToken);
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

        private static void FireReconnect(string reconnectUrl, CancellationToken cancellationToken) => Task.Run(() => Utils.TryElseError(() => {
            var newSocket = new ClientWebSocket();
            var newId = ConnectSocketTo(newSocket, reconnectUrl, cancellationToken);
            Close(WebSocketCloseStatus.NormalClosure, "Reconnect message received.", false, cancellationToken);
            socket = newSocket;
            Id = newId;
            cancellationTokenSource = new();
            listenTask = Task.Run(() => ListenAction(cancellationTokenSource.Token), CancellationToken.None);
        }), cancellationToken);
    }
}
