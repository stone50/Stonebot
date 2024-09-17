namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models.EventSub_Message;
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
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

        public bool IsConnected { get; private set; } = false;

        public async Task<bool> Connect() {
            var config = await AppCache.Config.Get();
            return config is not null && await Connect($"wss://eventsub.wss.twitch.tv/ws?keepalive_timeout_seconds={config.SocketKeepaliveTimeout}");
        }

        public async Task<bool> Close() => await Close(CloseReason.Manual);

        public async Task<string?> GetId() => !IsConnected && !await Connect() ? null : id;

        public void SetNotificationHandler(string subscriptionType, Func<JsonElement, Task> handler) => notificationHandlers[subscriptionType] = handler;

        public bool RemoveNotificationHandler(string subscriptionType) => notificationHandlers.Remove(subscriptionType);

        private string? id;
        private ClientWebSocket socket = new();
        private readonly Dictionary<string, Func<JsonElement, Task>> notificationHandlers = new();
        private DateTime keepaliveExpiration;

        private async Task<bool> Connect(string uri) {
            var config = await AppCache.Config.Get();
            if (config is null) {
                return false;
            }

            Uri socketUri;
            try {
                socketUri = new(uri);
            } catch (Exception e) {
                GD.PushWarning($"Cannot connect because Uri construction failed: {e}.");
                return false;
            }

            try {
                await socket.ConnectAsync(socketUri, default);
            } catch (Exception e) {
                GD.PushWarning($"Cannot connect because socket.ConnectAsync failed: {e}.");
                return false;
            }

            IsConnected = true;
            var request = await GetRequest();
            if (request is null) {
                _ = await Close(CloseReason.BadRequest);
                return false;
            }

            EventSubWelcomeMessage message;
            try {
                message = JsonSerializer.Deserialize<EventSubWelcomeMessage>(request);
            } catch (Exception e) {
                GD.PushWarning($"Cannot connect because JsonSerializer.Deserialize failed: {e}.");
                _ = await Close(CloseReason.BadRequest);
                return false;
            }

            id = message.Payload.Session.Id;
            keepaliveExpiration = DateTime.Now.AddSeconds(message.Payload.Session.KeepaliveTimeoutSeconds ?? config.SocketKeepaliveTimeout);
            _ = StartListening();
            return true;
        }

        private async Task<bool> Close(CloseReason reason) {
            if (!IsConnected) {
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
            try {
                await socket.CloseAsync(status, "", default);
            } catch (Exception e) {
                GD.PushError($"Cannot close because socket.CloseAsync failed: {e}.");
                return false;
            }

            socket = new();
            IsConnected = false;
            id = null;
            Closed.Invoke(this, reason);
            return true;
        }

        private async Task<string?> GetRequest() {
            var buffer = new byte[65536];
            WebSocketReceiveResult result;
            try {
                result = await socket.ReceiveAsync(buffer, default);
            } catch (Exception e) {
                GD.PushWarning($"Cannot get request because socket.ReceiveAsync failed: {e}.");
                return null;
            }

            if (result.CloseStatus is not null) {
                GD.PushWarning($"Web socket server sent close request: {result.CloseStatus}: {result.CloseStatusDescription}.");
                _ = await Close(CloseReason.CloseMessage);
                return null;
            }

            try {
                return Encoding.Default.GetString(buffer, 0, result.Count);
            } catch (Exception e) {
                GD.PushWarning($"Cannot get request because Encoding.Default.GetString failed: {e}.");
                return null;
            }
        }

        private async Task StartListening() {
            var config = await AppCache.Config.Get();
            if (config is null) {
                _ = await Close(CloseReason.InternalError);
                return;
            }

            _ = Task.Run(async () => {
                while (IsConnected) {
                    if (DateTime.Now > keepaliveExpiration.AddMilliseconds(config.SocketKeepaliveBuffer)) {
                        _ = await Close(CloseReason.KeepaliveTimeout);
                        GD.Print("Keepalive timeout");
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

                    GD.PushWarning("Cannot handle request because request is not supported.");
                    _ = await Close(CloseReason.BadRequest);
                    return;
                }
            });
        }

        private static bool TryParseRequest<T>(string request, out T requestData) where T : struct {
            try {
                requestData = JsonSerializer.Deserialize<T>(request);
            } catch (Exception e) {
                GD.PushWarning($"Cannot parse request because JsonSerializer.Deserialize failed: {e}.");
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
                GD.PushWarning("Cannot handle reconnect message because url is null.");
                return;
            }

            _ = await Close(CloseReason.ReconnectMessage);
        }
    }
}
