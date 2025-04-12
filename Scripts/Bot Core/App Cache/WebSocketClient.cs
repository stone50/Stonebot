namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Core_Interface.EventSub;
    using Models.EventSub;
    using Models.EventSub_Message;
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal class WebSocketClient {
        public enum CloseReason {
            Manual,
            BadRequest,
            CloseMessage,
            ReconnectMessage,
            KeepaliveTimeout,
            InternalError
        }

        public event EventHandler<EventSubRevocationMessage> EventSubRevoked = delegate { };
        public event EventHandler<CloseReason> Closed = delegate { };

        public WebSocketState State => socket.State;

        public async Task<bool> Connect() {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(Connect)}";
            Logger.Info(logPrefix);

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return false;
            }

            if (!await ConnectTo($"wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds={config.SocketKeepaliveTimeout}")) {
                Logger.Warning($"{logPrefix} | {nameof(ConnectTo)} result is false.");
                return false;
            }

            return true;
        }

        public async Task<bool> Close() {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(Close)}";
            Logger.Info(logPrefix);

            if (!await Close(CloseReason.Manual)) {
                Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                return false;
            }

            return true;
        }

        public async Task<string?> GetId() {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(GetId)}";
            Logger.Info(logPrefix);

            if (id is not null) {
                return id;
            }

            if (!await Connect()) {
                Logger.Warning($"{logPrefix} | {nameof(Connect)} result is false.");
                return null;
            }

            return id;
        }

        public void SetNotificationHandler(string subscriptionType, Func<JsonElement, Task> handler) {
            Logger.Info($"{nameof(WebSocketClient)} | {nameof(SetNotificationHandler)}\n{nameof(subscriptionType)}: {subscriptionType}\n{nameof(handler)}: {handler}");

            notificationHandlers[subscriptionType] = handler;
        }

        public bool RemoveNotificationHandler(string subscriptionType) {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(RemoveNotificationHandler)}";
            Logger.Info($"{logPrefix}\n{nameof(subscriptionType)}: {subscriptionType}");

            if (!notificationHandlers.Remove(subscriptionType)) {
                Logger.Warning($"{logPrefix} | {nameof(notificationHandlers.Remove)} result is false.");
                return false;
            }

            return true;
        }

        private readonly struct WebSocketRequestResult(bool close, string message) {
            public readonly bool Close = close;
            public readonly string Message = message;
        }

        private string? id;
        private ClientWebSocket socket = new();
        private readonly Dictionary<string, Func<JsonElement, Task>> notificationHandlers = [];

        private async Task<bool> ConnectTo(string uri) {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(ConnectTo)}";
            Logger.Info($"{logPrefix}\n{nameof(uri)}: {uri}");

            if (socket.State != WebSocketState.None) {
                Logger.Warning($"{logPrefix} | {nameof(socket.State)} is not {WebSocketState.None}");
                return false;
            }

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return false;
            }

            Uri socketUri;
            try {
                socketUri = new(uri);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(Uri)} Constructor threw: {e}.");
                return false;
            }

            var connectCancellationTokenSource = new CancellationTokenSource();
            connectCancellationTokenSource.CancelAfter(config.SocketKeepaliveBuffer);
            try {
                await socket.ConnectAsync(socketUri, connectCancellationTokenSource.Token);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(socket.ConnectAsync)} threw: {e}.\n{nameof(socketUri)}: {socketUri}\n{nameof(connectCancellationTokenSource.Token)}: {connectCancellationTokenSource.Token}");
                return false;
            }

            var requestCancellationTokenSource = new CancellationTokenSource();
            requestCancellationTokenSource.CancelAfter(config.SocketKeepaliveBuffer);
            var potentialRequestResult = await GetRequest(requestCancellationTokenSource.Token);
            if (potentialRequestResult is null) {
                if (!await Close(CloseReason.BadRequest)) {
                    Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                }

                Logger.Warning($"{logPrefix} | {nameof(GetRequest)} result is null.");
                return false;
            }

            var requestResult = (WebSocketRequestResult)potentialRequestResult;
            if (requestResult.Close) {
                if (!await Close(CloseReason.CloseMessage)) {
                    Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                }

                Logger.Warning($"{logPrefix} | {nameof(GetRequest)} was unsuccessful.\n{nameof(requestResult)}: {JsonSerializer.Serialize(requestResult)}");
                return false;
            }

            EventSubWelcomeMessage message;
            try {
                message = JsonSerializer.Deserialize<EventSubWelcomeMessage>(requestResult.Message);
            } catch (Exception e) {
                if (!await Close(CloseReason.BadRequest)) {
                    Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                }

                Logger.Warning($"{logPrefix} | {nameof(JsonSerializer.Deserialize)} threw: {e}.\n{nameof(requestResult.Message)}: {requestResult.Message}");
                return false;
            }

            id = message.Payload.Session.Id;
            _ = Task.Run(Listen);
            return true;
        }

        private async Task Listen() {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(Listen)}";
            Logger.Info(logPrefix);

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return;
            }

            while (true) {
                var keepaliveCancellationTokenSource = new CancellationTokenSource();
                keepaliveCancellationTokenSource.CancelAfter(config.SocketKeepaliveTimeout * 1000 + config.SocketKeepaliveBuffer);
                var potentialRequestResult = await GetRequest(keepaliveCancellationTokenSource.Token);
                if (potentialRequestResult is null) {
                    if (!await Close(CloseReason.BadRequest)) {
                        Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                    }

                    return;
                }

                var requestResult = (WebSocketRequestResult)potentialRequestResult;
                if (requestResult.Close) {
                    if (socket.State != WebSocketState.Open) {
                        return;
                    }

                    if (!await Close(CloseReason.CloseMessage)) {
                        Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                        return;
                    }

                    return;
                }

                if (TryParseRequest<EventSubKeepaliveMessage>(requestResult.Message, out var keepaliveData) && keepaliveData.Metadata.MessageType == "session_keepalive") {
                    continue;
                }

                if (TryParseRequest<EventSubNotificationMessage>(requestResult.Message, out var notificationData) && notificationData.Metadata.MessageType == "notification") {
                    await HandleNotification(notificationData);
                    continue;
                }

                if (TryParseRequest<EventSubReconnectMessage>(requestResult.Message, out var reconnectData) && reconnectData.Metadata.MessageType == "session_reconnect") {
                    await HandleReconnect(reconnectData);
                    continue;
                }

                if (TryParseRequest<EventSubRevocationMessage>(requestResult.Message, out var revocationData) && revocationData.Metadata.MessageType == "revocation") {
                    EventSubRevoked.Invoke(this, revocationData);
                    continue;
                }

                if (!await Close(CloseReason.BadRequest)) {
                    Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                }

                Logger.Warning($"{logPrefix} | {nameof(GetRequest)} result is not supported.\n{nameof(requestResult)}: {JsonSerializer.Serialize(requestResult)}");
                return;
            }
        }

        private async Task<bool> Close(CloseReason reason) {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(Close)}";
            Logger.Info($"{logPrefix}\n{nameof(reason)}: {reason}");

            if (socket.State != WebSocketState.Open) {
                Logger.Warning($"{logPrefix} | {nameof(socket.State)} is not {WebSocketState.Open}.");
                return false;
            }

            var status = reason switch {
                CloseReason.Manual => WebSocketCloseStatus.NormalClosure,
                CloseReason.BadRequest => WebSocketCloseStatus.InvalidMessageType,
                CloseReason.CloseMessage => WebSocketCloseStatus.NormalClosure,
                CloseReason.ReconnectMessage => WebSocketCloseStatus.NormalClosure,
                CloseReason.KeepaliveTimeout => WebSocketCloseStatus.NormalClosure,
                _ => WebSocketCloseStatus.InternalServerError,
            };
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(Constants.WebSocketClientCloseTimeout);
            var statusDescription = "";
            try {
                await socket.CloseAsync(status, statusDescription, cancellationTokenSource.Token);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(socket.CloseAsync)} threw: {e}.\n{nameof(status)}: {status}\n{nameof(statusDescription)}: {statusDescription}\n{nameof(cancellationTokenSource.Token)}: {cancellationTokenSource.Token}");
                return false;
            }

            socket = new();
            id = null;
            Closed.Invoke(this, reason);
            return true;
        }

        private async Task<WebSocketRequestResult?> GetRequest(CancellationToken cancellationToken) {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(GetRequest)}";
            Logger.Info($"{logPrefix}\n{nameof(cancellationToken)}: {cancellationToken}");

            var buffer = new byte[65536];
            WebSocketReceiveResult result;
            try {
                result = await socket.ReceiveAsync(buffer, cancellationToken);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(socket.ReceiveAsync)} threw: {e}.\n{nameof(buffer)}: {JsonSerializer.Serialize(buffer)}");
                return null;
            }

            if (result.CloseStatus is not null) {
                return new(true, "");
            }

            var index = 0;
            string message;
            try {
                message = Encoding.Default.GetString(buffer, index, result.Count);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(Encoding.Default.GetString)} threw: {e}.\n{nameof(buffer)}: {JsonSerializer.Serialize(buffer)}\n{nameof(index)}: {index}\n{nameof(result.Count)}: {result.Count}");
                return null;
            }

            return new(false, message);
        }

        private static bool TryParseRequest<T>(string request, out T requestData) where T : struct {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(TryParseRequest)}";
            Logger.Info($"{logPrefix}\n{nameof(request)}: {request}");

            try {
                requestData = JsonSerializer.Deserialize<T>(request);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(JsonSerializer.Deserialize)} threw: {e}.");
                requestData = default;
                return false;
            }

            return true;
        }

        private async Task HandleNotification(EventSubNotificationMessage message) {
            Logger.Info($"{nameof(WebSocketClient)} | {nameof(HandleNotification)}\n{nameof(message)}: {JsonSerializer.Serialize(message)}");

            if (!notificationHandlers.TryGetValue(message.Payload.Subscription.Type, out var handler)) {
                return;
            }

            await handler(message.Payload.Event);
        }

        private async Task HandleReconnect(EventSubReconnectMessage message) {
            var logPrefix = $"{nameof(WebSocketClient)} | {nameof(HandleReconnect)}";
            Logger.Info($"{logPrefix}\n{nameof(message)}: {JsonSerializer.Serialize(message)}");

            var url = message.Payload.Session.ReconnectUrl;
            if (url is null) {
                Logger.Warning($"{logPrefix} | {nameof(url)} is null.");
                return;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
                return;
            }

            var potentialEventSubs = await EventSub.Get(null, null, broadcaster.Id);
            if (potentialEventSubs is null) {
                Logger.Warning($"{logPrefix} | {nameof(EventSub.Get)} result is null.");
                return;
            }

            var eventSubs = (EventSubsData)potentialEventSubs;
            if (!await Close(CloseReason.ReconnectMessage)) {
                Logger.Warning($"{logPrefix} | {nameof(Close)} result is false.");
                return;
            }

            if (!await ConnectTo(url)) {
                Logger.Warning($"{logPrefix} | {nameof(ConnectTo)} result is false.");
                return;
            }

            if (id is null) {
                Logger.Warning($"{logPrefix} | {nameof(id)} is null.");
                return;
            }

            foreach (var eventSub in eventSubs.Data) {
                var newEventSub = eventSub;
                var newEventSubTransport = newEventSub.Transport;
                newEventSubTransport.SessionId = id;
                newEventSub.Transport = newEventSubTransport;
                if (!await EventSub.Add(newEventSub)) {
                    Logger.Warning($"{logPrefix} | {nameof(EventSub.Add)} result is false.\n{nameof(newEventSub)}: {JsonSerializer.Serialize(newEventSub)}");
                }
            }
        }
    }
}
