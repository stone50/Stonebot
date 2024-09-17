namespace StoneBot.Scripts.Core_Interface.EventSub {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Godot;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class ChannelChatMessage {
        public static async Task<bool> Connect(Func<ChannelChatMessageEvent, Task> handler) {
            var config = await AppCache.Config.Get();
            if (config is null) {
                return false;
            }

            var clientWrapper = await AppCache.HttpClientWrapper.Get();
            if (clientWrapper is null) {
                return false;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                return false;
            }

            var webSocketClient = await AppCache.WebSocketClient.Get();
            if (webSocketClient is null) {
                return false;
            }

            var sessionId = await webSocketClient.GetId();
            if (sessionId is null) {
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                return false;
            }

            var eventSubData = await Util.GetMessageAs<EventSubData>(TwitchAPI.SubscribeToChannelChatMessage(
                client,
                broadcaster.Id,
                broadcaster.Id,
                sessionId
            ));
            if (eventSubData is null) {
                return false;
            }

            webSocketClient.SetNotificationHandler("channel.chat.message", async (eventElement) => {
                ChannelChatMessageEvent messageEvent;
                try {
                    messageEvent = JsonSerializer.Deserialize<ChannelChatMessageEvent>(eventElement);
                } catch (Exception e) {
                    GD.PushWarning($"Cannot handle channel chat message event because JsonSerializer.Deserialize failed: {e}.");
                    return;
                }

                await handler(messageEvent);
            });
            return true;
        }
    }
}
