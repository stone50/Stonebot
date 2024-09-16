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
            var configValues = await AppCache.ConfigValues.Get();
            if (configValues is null) {
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

            var eventSubWebSocketClient = await AppCache.EventSubWebSocketClient.Get();
            if (eventSubWebSocketClient is null) {
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                return false;
            }

            var eventSubData = await Util.GetMessageAs<EventSubData>(TwitchAPI.ChannelChatMessageEventSub(
                client,
                broadcaster.Id,
                broadcaster.Id,
                eventSubWebSocketClient.Id
            ));
            if (eventSubData is null) {
                return false;
            }

            eventSubWebSocketClient.SetNotificationHandler("channel.chat.message", async (eventElement) => {
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
