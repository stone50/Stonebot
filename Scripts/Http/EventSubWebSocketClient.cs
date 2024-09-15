namespace StoneBot.Scripts.Http {
    using Godot;
    using Models.EventSub_Message;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using static WebSocketClient;

    internal class EventSubWebSocketClient {
        public string Id { get; private set; } = null!;

        private WebSocketClient Client = null!;

        // TODO: reconnect when keepalive_timeout_seconds is up

        private EventSubWebSocketClient() { }

        public static async Task<EventSubWebSocketClient?> Create() {
            var eventSubWebSocketClient = new EventSubWebSocketClient();
            var getIdPromise = new TaskCompletionSource<bool>();
            var setIdFromRequest = GetSetIdFromRequest(eventSubWebSocketClient, getIdPromise);
            eventSubWebSocketClient.Client = new();
            eventSubWebSocketClient.Client.RequestHandler += setIdFromRequest;
            if (!await eventSubWebSocketClient.Client.Start("wss://eventsub.wss.twitch.tv/ws")) {
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

        private void HandleRequest(object sender, string request) =>
            // TODO
            GD.Print(request);
    }
}
