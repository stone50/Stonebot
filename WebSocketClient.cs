namespace Stonebot {
    using Models;
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class WebSocketClient {
        public enum CloseReason {
            Manual,
            BadRequest,
            CloseMessage,
            ReconnectMessage,
            KeepaliveTimeout,
            InternalError
        }

        public static string? Id { get; private set; }

        public static Task ConnectAsync(CancellationToken cancellationToken) {
            if (Id is not null || socket is not null || listenTask is not null || cancellationTokenSource is not null) {
                Logger.Warn("Web socket client is already connected.");
                return Task.CompletedTask;
            }

            var url = Utils.GetUrl("wss://eventsub.wss.twitch.tv/ws", new() { { "keepalive_timeout_seconds", Config.SocketKeepaliveTimeoutSeconds.ToString() } });
            return ConnectToAsync(url, cancellationToken);
        }

        public static Task CloseAsync(CancellationToken cancellationToken) => CloseAsync(CloseReason.Manual, cancellationToken);

        private static ClientWebSocket? socket;
        private static Task? listenTask;
        private static CancellationTokenSource? cancellationTokenSource;

        private static async Task ConnectToAsync(string url, CancellationToken cancellationToken) {
            socket = new();
            try {
                Id = await ConnectSocketToAsync(socket, url, cancellationToken).ConfigureAwait(false);
                cancellationTokenSource = new();
                listenTask = new Task(ListenAction, cancellationTokenSource.Token);
                listenTask.Start();
            } catch {
                socket?.Dispose();
                socket = null;
            }
        }

        private static async Task<string> ConnectSocketToAsync(ClientWebSocket socket, string url, CancellationToken cancellationToken) {
            var socketUri = new Uri(url);
            await socket.ConnectAsync(socketUri, cancellationToken).ConfigureAwait(false);
            string? request;
            try {
                request = await GetRequestAsync(socket, cancellationToken).ConfigureAwait(false);
            } catch (OperationCanceledException) {
                // TODO: create different cancellation token
                await CloseSocketAsync(socket, CloseReason.Manual, cancellationToken).ConfigureAwait(false);
                throw;
            } catch (Exception) {
                await CloseSocketAsync(socket, CloseReason.BadRequest, cancellationToken).ConfigureAwait(false);
                throw;
            }

            if (request is null) {
                await CloseSocketAsync(socket, CloseReason.CloseMessage, cancellationToken).ConfigureAwait(false);
                throw new Exception("Received close message.");
            }

            var welcomeMessage = JsonSerializer.Deserialize(request, JsonContext.Default.EventSubWelcomeMessage);
            return welcomeMessage.Payload.Session.Id;
        }

        private static async void ListenAction() {
            while (cancellationTokenSource is not null && !cancellationTokenSource.IsCancellationRequested) {
                try {
                    await ListenAsync().ConfigureAwait(false);
                } catch (OperationCanceledException) {
                    // TODO: create different cancellation token
                    await CloseAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    return;
                } catch (Exception e) {
                    Logger.Error(e);
                    await CloseAsync(CloseReason.InternalError, cancellationTokenSource.Token).ConfigureAwait(false);
                    return;
                }
            }
        }

        private static async Task ListenAsync() {
            if (socket is null || cancellationTokenSource is null) {
                return;
            }

            // TODO: combine cancellationTokenSource with the keepalive timeout
            var request = await GetRequestAsync(socket, cancellationTokenSource.Token).ConfigureAwait(false);
            if (request is null) {
                await CloseAsync(CloseReason.CloseMessage, cancellationTokenSource.Token).ConfigureAwait(false);
                return;
            }

            Logger.Debug(request);

            if (TryParseRequest(request, JsonContext.Default.EventSubKeepaliveMessage, out var keepaliveData) && keepaliveData.Metadata.MessageType == "session_keepalive") {
                return;
            }

            if (TryParseRequest(request, JsonContext.Default.EventSubNotificationMessage, out var notificationData) && notificationData.Metadata.MessageType == "notification") {
                HandleNotification(notificationData);
                return;
            }

            if (TryParseRequest(request, JsonContext.Default.EventSubReconnectMessage, out var reconnectData) && reconnectData.Metadata.MessageType == "session_reconnect") {
                await HandleReconnectAsync(reconnectData).ConfigureAwait(false);
                return;
            }

            if (TryParseRequest(request, JsonContext.Default.EventSubRevocationMessage, out var revocationData) && revocationData.Metadata.MessageType == "revocation") {
                // TODO handle event sub revoked
                return;
            }
        }

        private static async Task CloseAsync(CloseReason reason, CancellationToken cancellationToken) {
            if (cancellationTokenSource is not null) {
                await cancellationTokenSource.CancelAsync().ConfigureAwait(false);
            }

            if (listenTask is not null) {
                await listenTask.ConfigureAwait(false);
                listenTask.Dispose();
                listenTask = null;
            }

            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            if (socket is not null) {
                if (socket.State == WebSocketState.Open) {
                    await CloseSocketAsync(socket, reason, cancellationToken).ConfigureAwait(false);
                }

                socket.Dispose();
                socket = null;
            }

            Id = null;
        }

        private static Task CloseSocketAsync(ClientWebSocket socket, CloseReason reason, CancellationToken cancellationToken) {
            var status = reason switch {
                CloseReason.Manual => WebSocketCloseStatus.NormalClosure,
                CloseReason.BadRequest => WebSocketCloseStatus.InvalidMessageType,
                CloseReason.CloseMessage => WebSocketCloseStatus.NormalClosure,
                CloseReason.ReconnectMessage => WebSocketCloseStatus.NormalClosure,
                CloseReason.KeepaliveTimeout => WebSocketCloseStatus.NormalClosure,
                _ => WebSocketCloseStatus.InternalServerError,
            };
            return socket.CloseAsync(status, "", cancellationToken);
        }

        private static async Task<string?> GetRequestAsync(ClientWebSocket socket, CancellationToken cancellationToken) {
            var buffer = new byte[65536];
            var result = await socket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
            return result.CloseStatus is not null ? null : Encoding.UTF8.GetString(buffer, 0, result.Count);
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

        private static void HandleNotification(EventSubNotificationMessage message) => _ = message.Payload.Subscription.Type;// TODO

        private static async Task HandleReconnectAsync(EventSubReconnectMessage message) {
            if (cancellationTokenSource is null) {
                return;
            }

            var newSocket = new ClientWebSocket();
            var newId = await ConnectSocketToAsync(newSocket, message.Payload.Session.ReconnectUrl!, cancellationTokenSource.Token).ConfigureAwait(false);
            await CloseAsync(CloseReason.ReconnectMessage, cancellationTokenSource.Token).ConfigureAwait(false);
            socket?.Dispose();
            socket = newSocket;
            Id = newId;
            await cancellationTokenSource.CancelAsync().ConfigureAwait(false);
            if (listenTask is not null) {
                await listenTask.ConfigureAwait(false);
                listenTask.Dispose();
            }

            cancellationTokenSource = new();
            listenTask = new Task(ListenAction, cancellationTokenSource.Token);
            listenTask.Start();
        }
    }
}
