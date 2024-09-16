namespace StoneBot.Scripts.Bot_Core.Http {
    using Godot;
    using Models.EventSub_Message;
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using static WebSocketClient;

    internal class EventSubWebSocketClient {
        public string Id { get; private set; } = null!;

        private WebSocketClient Client = null!;
        private readonly Dictionary<string, Func<JsonElement, Task>> NotificationHandlers = new();

        // TODO: refactor

        private EventSubWebSocketClient() { }

        public static async Task<EventSubWebSocketClient?> Create() => await Create("wss://eventsub.wss.twitch.tv/ws");

        private static async Task<EventSubWebSocketClient?> Create(string url) {
            var eventSubWebSocketClient = new EventSubWebSocketClient();
            var getIdPromise = new TaskCompletionSource<bool>();
            var setIdFromRequest = GetSetIdFromRequest(eventSubWebSocketClient, getIdPromise);
            eventSubWebSocketClient.Client = new();
            eventSubWebSocketClient.Client.RequestHandler += setIdFromRequest;
            if (!await eventSubWebSocketClient.Client.Start(url)) {
                GD.PushWarning("Cannot create EventSubWebSocketClient because Client.Start failed.");
                return null;
            }

            var success = await getIdPromise.Task;
            eventSubWebSocketClient.Client.RequestHandler -= setIdFromRequest;
            if (!success) {
                GD.PushWarning("Cannot create EventSubWebSocketClient because setIdFromRequest failed.");
                if (!await eventSubWebSocketClient.Client.Stop()) {
                    GD.PushError("EventSubWebSocketClient stop failed.");
                }

                return null;
            }

            eventSubWebSocketClient.Client.RequestHandler += eventSubWebSocketClient.HandleRequest;

            return eventSubWebSocketClient;
        }

        public void SetNotificationHandler(string subscriptionType, Func<JsonElement, Task> handler) => NotificationHandlers[subscriptionType] = handler;

        public bool RemoveNotificationHandler(string subscriptionType) => NotificationHandlers.Remove(subscriptionType);

        private static WebSocketRequestHandler GetSetIdFromRequest(EventSubWebSocketClient eventSubWebSocketClient, TaskCompletionSource<bool> promise) => (object sender, string request) => {
            EventSubWelcomeMessage welcomeMessage;
            try {
                welcomeMessage = JsonSerializer.Deserialize<EventSubWelcomeMessage>(request);
            } catch (Exception e) {
                GD.PushWarning($"Could not parse request: {e}.");
                if (!promise.TrySetResult(false)) {
                    GD.PushError("Could not resolve promise.");
                }

                return;
            }

            eventSubWebSocketClient.Id = welcomeMessage.Payload.Session.Id;
            if (!promise.TrySetResult(true)) {
                GD.PushError("Could not resolve promise.");
            }
        };

        private void HandleRequest(object sender, string request) => _ = Task.Run(async () => {
            if (TryParseRequest<EventSubKeepaliveMessage>(request, out var keepaliveData) && keepaliveData.Metadata.MessageType == "session_keepalive") {
                return;
            }

            if (TryParseRequest<EventSubNotificationMessage>(request, out var notificationData) && notificationData.Metadata.MessageType == "notification") {
                await HandleNotificationMessage(notificationData);
                return;
            }

            if (TryParseRequest<EventSubReconnectMessage>(request, out var reconnectData) && reconnectData.Metadata.MessageType == "session_reconnect") {
                await HandleReconnectMessage(reconnectData);
                return;
            }

            if (TryParseRequest<EventSubRevocationMessage>(request, out var revocationData) && revocationData.Metadata.MessageType == "revocation") {
                HandleRevocationMessage(revocationData);
                return;
            }

            GD.PushWarning("Cannot handle web socket request because request could not be parsed.");
        });

        private static bool TryParseRequest<T>(string request, out T requestData) where T : struct {
            try {
                requestData = JsonSerializer.Deserialize<T>(request);
            } catch {
                requestData = default;
                return false;
            }

            return true;
        }

        private async Task HandleNotificationMessage(EventSubNotificationMessage message) {
            if (!NotificationHandlers.TryGetValue(message.Payload.Subscription.Type, out var handler)) {
                return;
            }

            await handler(message.Payload.Event);
        }

        private async Task HandleReconnectMessage(EventSubReconnectMessage message) {
            var reconnectUrl = message.Payload.Session.ReconnectUrl;
            if (reconnectUrl is null) {
                GD.PushWarning("Cannot reconnect web socket client because ReconnectUrl is null.");
                return;
            }

            if (!await Reconnect(reconnectUrl)) {
                GD.PushWarning("Cannot reconnect web socket because Reconnect failed.");
            }
        }

        private static void HandleRevocationMessage(EventSubRevocationMessage message) => GD.Print($"Subscription revoked: id={message.Payload.Subscription.Id} type={message.Payload.Subscription.Type}");

        private async Task<bool> Reconnect(string reconnectUrl) {
            var newClient = await Create(reconnectUrl);
            if (newClient is null) {
                GD.PushWarning("Cannot reconnect web socket client because Create failed.");
                return false;
            }

            Id = newClient.Id;
            Client = newClient.Client;

            Client.RequestHandler -= newClient.HandleRequest;
            Client.RequestHandler += HandleRequest;
            return true;
        }
    }
}
