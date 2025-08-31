namespace Stonebot {
    using Helpers;
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Threading;
    using System.Threading.Tasks;
    using Twitch;

    internal static class WebSocketClient {
        public static event EventHandler FullClosure = delegate { };

        public static string? Id { get; private set; }

        public static void Connect() {
            var url = HttpHelper.GetUrl("wss://eventsub.wss.twitch.tv/ws", new() {
                { "keepalive_timeout_seconds", Constants.WebSocketKeepaliveTimeoutSecs.ToString() }
            });
            ConnectTo(url);
            try {
                EventSub.SubscribeToChannelChatMessage();
            } catch {
                Close(WebSocketCloseStatus.InternalServerError, "Error while subscribing to event.", true);
                throw;
            }
        }

        public static void Close() {
            Close(WebSocketCloseStatus.NormalClosure, null, true);
            EventSub.DeleteEventSubs();
        }

        private static ClientWebSocket? socket;
        private static Task? listenTask;
        private static CancellationTokenSource? cancellationTokenSource;

        private static void ConnectTo(string url) {
            socket = new();
            Id = ConnectSocketTo(socket, url);
            cancellationTokenSource = new();
            listenTask = Task.Run(ListenAction);
        }

        private static void Close(WebSocketCloseStatus status, string? statusDescription, bool isFullClosure) {
            cancellationTokenSource?.Cancel();
            if (listenTask != null) {
                TaskHelper.Sync(listenTask);
            }

            listenTask = null;
            cancellationTokenSource = null;
            if (socket != null) {
                if (socket.State == WebSocketState.Open) {
                    var cancellationToken = TaskHelper.GetDefaultCancellationToken();
                    TaskHelper.Sync(socket.CloseAsync(status, statusDescription, cancellationToken));
                }

                socket = null;
            }

            Id = null;
            if (isFullClosure) {
                EventSub.DeleteEventSubs();
                FullClosure.Invoke(null, new());
            }
        }

        private static string ConnectSocketTo(ClientWebSocket socket, string url) {
            var socketUri = new Uri(url);
            var cancellationToken = TaskHelper.GetDefaultCancellationToken();
            TaskHelper.Sync(socket.ConnectAsync(socketUri, cancellationToken));
            try {
                var requestCancellationToken = TaskHelper.GetDefaultCancellationToken();
                var request = GetRequest(socket, requestCancellationToken);
                var welcomeMessage = JsonSerializer.Deserialize(request!, JsonContext.Default.EventSubWelcomeMessage);
                return welcomeMessage.Payload.Session.Id;
            } catch (Exception e) {
                var closeCancellationToken = TaskHelper.GetDefaultCancellationToken();
                TaskHelper.Sync(socket.CloseAsync(WebSocketCloseStatus.InternalServerError, e.Message, closeCancellationToken));
                throw;
            }
        }

        private static void ListenAction() {
            if (socket == null) {
                return;
            }

            if (cancellationTokenSource == null) {
                return;
            }

            while (!cancellationTokenSource.IsCancellationRequested) {
                var timeoutCancellationTokenSource = new CancellationTokenSource();
                var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutCancellationTokenSource.Token);
                timeoutCancellationTokenSource.CancelAfter((Constants.WebSocketKeepaliveTimeoutSecs + Constants.WebSocketKeepaliveTimeoutMarginSecs) * 1000);
                string? request;
                try {
                    request = GetRequest(socket, linkedCancellationTokenSource.Token);
                } catch (OperationCanceledException) {
                    if (timeoutCancellationTokenSource.IsCancellationRequested) {
                        Logger.Warn("No keepalive message received.");
                        FireClose(WebSocketCloseStatus.NormalClosure, "No keepalive message received.", true);
                    }

                    return;
                } catch (Exception e) {
                    Logger.Error(e);
                    FireClose(WebSocketCloseStatus.InternalServerError, e.Message, true);
                    return;
                }

                if (request == null) {
                    FireClose(WebSocketCloseStatus.NormalClosure, "Close message received.", true);
                    return;
                }

                if (
                    TryParseRequest(request, JsonContext.Default.EventSubKeepaliveMessage, out var keepaliveMessage) &&
                    keepaliveMessage.Metadata.MessageType == "session_keepalive"
                ) {
                    continue;
                }

                if (
                    TryParseRequest(request, JsonContext.Default.EventSubNotificationMessage, out var notificationMessage) &&
                    notificationMessage.Metadata.MessageType == "notification"
                ) {
                    try {
                        ChatMessageHandler.HandleChatMessage(notificationMessage.Payload.Event);
                    } catch (Exception e) {
                        Logger.Error(e);
                    }

                    continue;
                }

                if (
                    TryParseRequest(request, JsonContext.Default.EventSubReconnectMessage, out var reconnectMessage) &&
                    reconnectMessage.Metadata.MessageType == "session_reconnect"
                ) {
                    FireReconnect(reconnectMessage.Payload.Session.ReconnectUrl);
                    continue;
                }

                if (
                    TryParseRequest(request, JsonContext.Default.EventSubRevocationMessage, out var revocationMessage) &&
                    revocationMessage.Metadata.MessageType == "revocation"
                ) {
                    var subscription = revocationMessage.Payload.Subscription;
                    try {
                        EventSub.DeleteEventSub(subscription.Id);
                    } catch (Exception e) {
                        Logger.Error(e);
                    }

                    Logger.Warn("Event subscription revoked.", subscription.Status);
                    FireClose(WebSocketCloseStatus.NormalClosure, "Event subscription revoked.", true);
                    return;
                }

                Logger.Warn("Received invalid message type.");
                FireClose(WebSocketCloseStatus.InvalidMessageType, null, true);
                return;

            }
        }

        private static string? GetRequest(ClientWebSocket socket, CancellationToken cancellationToken) {
            var buffer = new byte[Constants.WebSocketRequestBufferLength];
            var result = TaskHelper.Sync(socket.ReceiveAsync(buffer, cancellationToken));
            return result.MessageType == WebSocketMessageType.Close ? null : Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        private static void FireClose(WebSocketCloseStatus status, string? statusDescription, bool isFullClosure) => TaskHelper.FireTryElseError(() => Close(status, statusDescription, isFullClosure));

        private static bool TryParseRequest<T>(string request, JsonTypeInfo<T> jsonTypeInfo, out T requestData) where T : struct {
            try {
                requestData = JsonSerializer.Deserialize(request, jsonTypeInfo);
            } catch {
                requestData = default;
                return false;
            }

            return true;
        }

        private static void FireReconnect(string reconnectUrl) => TaskHelper.FireTryElseError(() => {
            var newSocket = new ClientWebSocket();
            var newId = ConnectSocketTo(newSocket, reconnectUrl);
            Close(WebSocketCloseStatus.NormalClosure, "Reconnect message received.", false);
            socket = newSocket;
            Id = newId;
            cancellationTokenSource = new();
            listenTask = Task.Run(ListenAction);
        });
    }
}
