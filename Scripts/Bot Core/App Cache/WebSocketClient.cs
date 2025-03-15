namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Core_Interface.EventSub;
    using Models.EventSub;
    using Models.EventSub_Message;
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    // TODO: This whole class needs to be rewritten. The flow of data is unclear and prone to bugs.
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

        public bool IsConnected { get; private set; } = false;
        public bool IsClosing { get; private set; } = false;

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

            await StartListening();
            return true;
        }

        public async Task<bool> Close() => await Close(CloseReason.Manual);

        public async Task<string?> GetId() {
            Logger.Info("Getting web socket client id.");

            if (!IsConnected && !await Connect()) {
                Logger.Warning("Could not get web socket client id because the web socket client is not connected and the connect attempt failed.");
                return null;
            }

            return id;
        }

        public void SetNotificationHandler(string subscriptionType, Func<JsonElement, Task> handler) => notificationHandlers[subscriptionType] = handler;

        public bool RemoveNotificationHandler(string subscriptionType) => notificationHandlers.Remove(subscriptionType);

        private string? id;
        private ClientWebSocket socket = new();
        private readonly Dictionary<string, Func<JsonElement, Task>> notificationHandlers = [];
        private DateTime keepaliveExpiration;

        private async Task<bool> ConnectTo(string uri) {
            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not connect web socket client because config get attempt failed.");
                return false;
            }

            Uri socketUri;
            try {
                socketUri = new(uri);
            } catch (Exception e) {
                Logger.Warning($"Could not connect web socket client because uri construct attempt failed: {e}. Context value: {uri}.");
                return false;
            }

            try {
                await socket.ConnectAsync(socketUri, default);
            } catch (Exception e) {
                Logger.Warning($"Could not connect web socket client because socket connect attempt failed: {e}. Context value: {socketUri}.");
                return false;
            }

            IsConnected = true;
            var request = await GetRequest();
            if (request is null) {
                if (!await Close(CloseReason.BadRequest)) {
                    Logger.Warning("Close attempt failed.");
                }

                Logger.Warning("Could not connect web socket client because get request attempt failed.");
                return false;
            }

            EventSubWelcomeMessage message;
            try {
                message = JsonSerializer.Deserialize<EventSubWelcomeMessage>(request);
            } catch (Exception e) {
                if (!await Close(CloseReason.BadRequest)) {
                    Logger.Warning("Close attempt failed.");
                }

                Logger.Warning($"Could not connect web socket client because json serialize deserialize attempt failed: {e}. Context value: {request}.");
                return false;
            }

            id = message.Payload.Session.Id;
            keepaliveExpiration = DateTime.Now.AddSeconds(message.Payload.Session.KeepaliveTimeoutSeconds ?? config.SocketKeepaliveTimeout);
            return true;
        }

        private async Task<bool> Close(CloseReason reason) {
            if (!IsConnected) {
                Logger.Warning("Could not close web socket client because the web socket client is not connected.");
                return false;
            }

            if (IsClosing) {
                Logger.Warning("Could not close web socket client because the web socket client is closing.");
                return false;
            }

            IsClosing = true;
            var status = reason switch {
                CloseReason.Manual => WebSocketCloseStatus.NormalClosure,
                CloseReason.BadRequest => WebSocketCloseStatus.InvalidMessageType,
                CloseReason.CloseMessage => WebSocketCloseStatus.NormalClosure,
                CloseReason.ReconnectMessage => WebSocketCloseStatus.NormalClosure,
                CloseReason.KeepaliveTimeout => WebSocketCloseStatus.NormalClosure,
                _ => WebSocketCloseStatus.InternalServerError,
            };
            try {
                await socket.CloseAsync(status, "", default);
            } catch (Exception e) {
                IsClosing = false;
                Logger.Warning($"Could not close web socket client because socket close attempt failed: {e}.");
                return false;
            }

            IsConnected = false;
            socket = new();
            id = null;
            IsClosing = false;
            Closed.Invoke(this, reason);
            return true;
        }

        private async Task<string?> GetRequest() {
            var buffer = new byte[65536];
            WebSocketReceiveResult result;
            try {
                result = await socket.ReceiveAsync(buffer, default);
            } catch (Exception e) {
                Logger.Warning($"Could not get request because socket receive attempt failed: {e}.");
                return null;
            }

            if (result.CloseStatus is not null) {
                if (!await Close(CloseReason.CloseMessage)) {
                    Logger.Warning("Close attempt failed.");
                }

                Logger.Warning($"Could not get request because socket recieve attempt failed. Context value: {result.CloseStatus}.");
                return null;
            }

            try {
                return Encoding.Default.GetString(buffer, 0, result.Count);
            } catch (Exception e) {
                Logger.Warning($"Could not get request because encoding default get string failed: {e}.");
                return null;
            }
        }

        private async Task StartListening() {
            var config = await AppCache.Config.Get();
            if (config is null) {
                if (!await Close(CloseReason.InternalError)) {
                    Logger.Warning("Close attempt failed.");
                }

                Logger.Warning("Could not start listening");
                return;
            }

            _ = Task.Run(async () => {
                while (IsConnected) {
                    if (DateTime.Now > keepaliveExpiration.AddMilliseconds(config.SocketKeepaliveBuffer)) {
                        _ = await Close(CloseReason.KeepaliveTimeout);
                        return;
                    }
                }
            });

            _ = Task.Run(async () => {
                while (IsConnected) {
                    var request = await GetRequest();
                    if (request is null) {
                        _ = await Close(CloseReason.BadRequest);
                        return;
                    }

                    if (TryParseRequest<EventSubKeepaliveMessage>(request, out var keepaliveData) && keepaliveData.Metadata.MessageType == "session_keepalive") {
                        keepaliveExpiration = DateTime.Now.AddSeconds(config.SocketKeepaliveTimeout);
                        continue;
                    }

                    if (TryParseRequest<EventSubNotificationMessage>(request, out var notificationData) && notificationData.Metadata.MessageType == "notification") {
                        keepaliveExpiration = DateTime.Now.AddSeconds(config.SocketKeepaliveTimeout);
                        await HandleNotification(notificationData);
                        continue;
                    }

                    if (TryParseRequest<EventSubReconnectMessage>(request, out var reconnectData) && reconnectData.Metadata.MessageType == "session_reconnect") {
                        keepaliveExpiration = DateTime.Now.AddSeconds(reconnectData.Payload.Session.KeepaliveTimeoutSeconds ?? config.SocketKeepaliveTimeout);
                        await HandleReconnect(reconnectData);
                        continue;
                    }

                    if (TryParseRequest<EventSubRevocationMessage>(request, out var revocationData) && revocationData.Metadata.MessageType == "revocation") {
                        keepaliveExpiration = DateTime.Now.AddSeconds(config.SocketKeepaliveTimeout);
                        EventSubRevoked.Invoke(this, revocationData);
                        continue;
                    }

                    Logger.Warning("Cannot handle request because request is not supported.");
                    _ = await Close(CloseReason.BadRequest);
                    return;
                }
            });
        }

        private static bool TryParseRequest<T>(string request, out T requestData) where T : struct {
            try {
                requestData = JsonSerializer.Deserialize<T>(request);
            } catch (Exception e) {
                Logger.Warning($"Cannot parse request because JsonSerializer.Deserialize failed: {e}.");
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
                Logger.Warning("Cannot handle reconnect message because url is null.");
                return;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                return;
            }

            var potentialEventSubs = await EventSub.Get(null, null, broadcaster.Id);
            if (potentialEventSubs is null) {
                return;
            }

            var eventSubs = (EventSubsData)potentialEventSubs;
            if (!await Close(CloseReason.ReconnectMessage)) {
                return;
            }

            if (!await ConnectTo(url)) {
                return;
            }

            if (id is null) {
                return;
            }

            foreach (var eventSub in eventSubs.Data) {
                var newEventSub = eventSub;
                var newEventSubTransport = newEventSub.Transport;
                newEventSubTransport.SessionId = id;
                newEventSub.Transport = newEventSubTransport;
                if (!await EventSub.Add(newEventSub)) {
                    return;
                }
            }
        }
    }
}
