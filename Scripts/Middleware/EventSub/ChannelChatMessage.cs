namespace StoneBot.Scripts.Middleware.EventSub {
    using App_Cache;
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal static class ChannelChatMessage {
        public static async Task<bool> Connect() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot connect channel chat message event sub because configValues is null.");
                return false;
            }

            var httpUserAccessTokenClient = await AppCache.HttpUserAccessTokenClient.Get();
            if (httpUserAccessTokenClient is null) {
                GD.PushWarning("Cannot connect channel chat message event sub because httpUserAccessTokenClient is null.");
                return false;
            }

            var broadcasterId = await AppCache.BroadcasterId.Get();
            if (broadcasterId is null) {
                GD.PushWarning("Cannot connect channel chat message event sub because broadcasterId is null.");
                return false;
            }

            var eventSubWebSocketClient = await AppCache.EventSubWebSocketClient.Get();
            if (eventSubWebSocketClient is null) {
                GD.PushWarning("Cannot connect channel chat message event sub because eventSubWebSocketClient is null.");
                return false;
            }

            var client = await httpUserAccessTokenClient.GetClient();
            if (client is null) {
                GD.PushWarning("Cannot connect channel chat message event sub because httpUserAccessTokenClient.GetClient failed.");
                return false;
            }

            var eventSubData = await Util.ProcessHttpResponseMessage<EventSubData>(await TwitchAPI.ChannelChatMessageEventSub(
                client,
                broadcasterId,
                broadcasterId,
                eventSubWebSocketClient.Id
            ));
            if (eventSubData is null) {
                GD.PushWarning("Cannot connect channel chat message event sub because ProcessHttpResponseMessage failed.");
                return false;
            }

            return true;
        }
    }
}
