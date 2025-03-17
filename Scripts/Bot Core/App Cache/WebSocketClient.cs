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
            Logger.Info("Connecting web socket client.");

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not connect web socket client because config get attempt failed.");
                return false;
            }

            if (!await ConnectTo($"wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds={config.SocketKeepaliveTimeout}")) {
                Logger.Warning("Could not connect web socket client because connect to attempt failed.");
                return false;
            }

            return true;
        }

        public async Task<bool> Close() {
            Logger.Info("Closing web socket client.");

            if (!await Close(CloseReason.Manual)) {
                Logger.Warning("Could not close web socket client because web socket client close attempt failed.");
                return false;
            }

            return true;
        }

        public async Task<string?> GetId() {
            Logger.Info("Getting web socket client id.");

            if (id is null && !await Connect()) {
                Logger.Warning("Could not get web socket client id because the web socket client id is null and the connect attempt failed.");
                return null;
            }

            return id;
        }

        public void SetNotificationHandler(string subscriptionType, Func<JsonElement, Task> handler) {
            Logger.Info($"Setting notification handler. Subscription type: {subscriptionType}.");

            notificationHandlers[subscriptionType] = handler;
        }

        public bool RemoveNotificationHandler(string subscriptionType) {
            Logger.Info($"Removing notification handler. Subscription type: {subscriptionType}.");

            if (!notificationHandlers.Remove(subscriptionType)) {
                Logger.Warning($"Could not remove notification handler because notification handlers remove attempt failed. Subscription type: {subscriptionType}.");
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
            if (socket.State != WebSocketState.None) {
                Logger.Warning("Could not connect web socket client because socket state is not none.");
                return false;
            }

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not connect web socket client because config get attempt failed.");
                return false;
            }

            Uri socketUri;
            try {
                socketUri = new(uri);
            } catch (Exception e) {
                Logger.Warning($"Could not connect web socket client because uri construct attempt failed: {e}. Uri: {uri}.");
                return false;
            }

            var connectCancellationTokenSource = new CancellationTokenSource();
            connectCancellationTokenSource.CancelAfter(config.SocketKeepaliveBuffer);
            try {
                await socket.ConnectAsync(socketUri, connectCancellationTokenSource.Token);
            } catch (Exception e) {
                Logger.Warning($"Could not connect web socket client because socket connect attempt failed: {e}. Socket uri: {socketUri}.");
                return false;
            }

            var requestCancellationTokenSource = new CancellationTokenSource();
            requestCancellationTokenSource.CancelAfter(config.SocketKeepaliveBuffer);
            var potentialRequestResult = await GetRequest(requestCancellationTokenSource.Token);
            if (potentialRequestResult is null) {
                if (!await Close(CloseReason.BadRequest)) {
                    Logger.Warning("Close attempt failed.");
                }

                Logger.Warning("Could not connect web socket client because get request attempt failed.");
                return false;
            }

            var requestResult = (WebSocketRequestResult)potentialRequestResult;
            if (requestResult.Close) {
                if (!await Close(CloseReason.CloseMessage)) {
                    Logger.Warning("Close attempt failed.");
                }

                Logger.Warning("Could not connect web socket client because close message was received.");
                return false;
            }

            EventSubWelcomeMessage message;
            try {
                message = JsonSerializer.Deserialize<EventSubWelcomeMessage>(requestResult.Message);
            } catch (Exception e) {
                if (!await Close(CloseReason.BadRequest)) {
                    Logger.Warning("Close attempt failed.");
                }

                Logger.Warning($"Could not connect web socket client because json serialize deserialize attempt failed: {e}. Request result message: {requestResult.Message}.");
                return false;
            }

            id = message.Payload.Session.Id;
            _ = Task.Run(async () => {
                while (true) {
                    var keepaliveCancellationTokenSource = new CancellationTokenSource();
                    keepaliveCancellationTokenSource.CancelAfter(config.SocketKeepaliveTimeout * 1000 + config.SocketKeepaliveBuffer);
                    var potentialRequestResult = await GetRequest(keepaliveCancellationTokenSource.Token);
                    if (potentialRequestResult is null) {
                        if (!await Close(CloseReason.BadRequest)) {
                            Logger.Warning("Web socket client close attempt failed.");
                        }

                        return;
                    }

                    var requestResult = (WebSocketRequestResult)potentialRequestResult;
                    if (requestResult.Close) {
                        if (socket.State == WebSocketState.Open && !await Close(CloseReason.CloseMessage)) {
                            Logger.Warning("Web socket client close attempt failed.");
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
                        Logger.Warning("Web socket client close attempt failed.");
                    }

                    Logger.Warning("Cannot handle request because request is not supported.");
                    return;
                }
            });

            return true;
        }

        private async Task<bool> Close(CloseReason reason) {
            if (socket.State != WebSocketState.Open) {
                Logger.Warning("Could not close web socket client because socket state is not open.");
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
            try {
                await socket.CloseAsync(status, "", cancellationTokenSource.Token);
            } catch (Exception e) {
                Logger.Warning($"Could not close web socket client because socket close attempt failed: {e}.");
                return false;
            }

            socket = new();
            id = null;
            Closed.Invoke(this, reason);
            return true;
        }

        private async Task<WebSocketRequestResult?> GetRequest(CancellationToken cancellationToken) {
            var buffer = new byte[65536];
            WebSocketReceiveResult result;
            try {
                result = await socket.ReceiveAsync(buffer, cancellationToken);
            } catch (Exception e) {
                Logger.Warning($"Could not get request because socket receive attempt failed: {e}.");
                return null;
            }

            if (result.CloseStatus is not null) {
                return new(true, "");
            }

            try {
                return new(false, Encoding.Default.GetString(buffer, 0, result.Count));
            } catch (Exception e) {
                Logger.Warning($"Could not get request because encoding default get string failed: {e}.");
                return null;
            }
        }

        private static bool TryParseRequest<T>(string request, out T requestData) where T : struct {
            try {
                requestData = JsonSerializer.Deserialize<T>(request);
            } catch (Exception e) {
                Logger.Warning($"Could not parse request because json serializer deserialize attempt failed: {e}. Request: {request}.");
                requestData = default;
                return false;
            }

            return true;
        }

        private async Task HandleNotification(EventSubNotificationMessage message) {
            if (!notificationHandlers.TryGetValue(message.Payload.Subscription.Type, out var handler)) {
                return;
            }

            await handler(message.Payload.Event);
        }

        private async Task HandleReconnect(EventSubReconnectMessage message) {
            var url = message.Payload.Session.ReconnectUrl;
            if (url is null) {
                Logger.Warning("Could not web socket client handle reconnect because url is null.");
                return;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not web socket client handle reconnect because broadcaster get attempt failed.");
                return;
            }

            var potentialEventSubs = await EventSub.Get(null, null, broadcaster.Id);
            if (potentialEventSubs is null) {
                Logger.Warning("Could not web socket client handle reconnect because event sub get attempt failed.");
                return;
            }

            var eventSubs = (EventSubsData)potentialEventSubs;
            if (!await Close(CloseReason.ReconnectMessage)) {
                Logger.Warning("Web socket client close attempt failed.");
                return;
            }

            if (!await ConnectTo(url)) {
                Logger.Warning($"Could not web socket client handle reconnect because connect attempt failed. Url: {url}.");
                return;
            }

            if (id is null) {
                Logger.Warning("Could not web socket client handle reconnect because the web socket client id is null.");
                return;
            }

            foreach (var eventSub in eventSubs.Data) {
                var newEventSub = eventSub;
                var newEventSubTransport = newEventSub.Transport;
                newEventSubTransport.SessionId = id;
                newEventSub.Transport = newEventSubTransport;
                if (!await EventSub.Add(newEventSub)) {
                    Logger.Warning("Web socket client event sub add attempt failed.");
                }
            }
        }
    }
}
