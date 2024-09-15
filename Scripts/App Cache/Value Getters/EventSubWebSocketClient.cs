namespace StoneBot.Scripts.App_Cache.Value_Getters {
    using Godot;
    using Http;
    using System.Threading.Tasks;

    internal static partial class ValueGetters {
        public static async Task<EventSubWebSocketClient?> GetEventSubWebSocketClient() {
            var eventSubWebSocketClient = await EventSubWebSocketClient.Create();
            if (eventSubWebSocketClient is null) {
                GD.PushWarning("Cannot get event sub web socket client because creation failed.");
                return null;
            }

            return eventSubWebSocketClient;
        }
    }
}
