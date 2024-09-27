namespace StoneBot.Scripts.Core_Interface.EventSub {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Bot_Core.Twitch;
    using System;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class EventSub {
        // only up to 1 of status, type, and userId should be specified
        public static async Task<EventSubsData?> Get(string? status = null, string? type = null, string? userId = null) {
            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                return null;
            }

            EventSubsData? combinedData = null;
            string? cursor = null;
            while (true) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    return null;
                }

                var potentialData = await Util.GetMessageAs<EventSubsData>(TwitchAPI.GetEventSubs(client, status, type, userId, cursor));
                if (potentialData is null) {
                    return null;
                }

                var data = (EventSubsData)potentialData;
                if (combinedData is null) {
                    combinedData = data;
                } else {
                    var newCombinedData = (EventSubsData)combinedData;
                    newCombinedData.Data = newCombinedData.Data.Concat(data.Data).ToArray();
                    combinedData = newCombinedData;
                }

                if (data.Pagination.Cursor is null) {
                    break;
                }

                cursor = data.Pagination.Cursor;
            }

            return combinedData;
        }

        // only up to 1 of status, type, and userId should be specified
        public static async Task<bool> RemoveBy(string? status = null, string? type = null, string? userId = null) {
            var potentialData = await Get(status, type, userId);
            return potentialData is not null && await Remove(((EventSubsData)potentialData).Data);
        }

        public static async Task<bool> Remove(EventSubData[] eventSubs) {
            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                return false;
            }

            foreach (var eventSub in eventSubs) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    return false;
                }

                var successfulString = await Util.GetSuccessfulString(TwitchAPI.DeleteEventSub(client, eventSub.Id));
                if (successfulString is null) {
                    return false;
                }
            }

            return true;
        }

        public static async Task<bool> ConnectChannelChatMessage(Func<ChannelChatMessageEvent, Task> handler) {
            var config = await AppCache.Config.Get();
            if (config is null) {
                return false;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                return false;
            }

            var bot = await AppCache.Bot.Get();
            if (bot is null) {
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

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                return false;
            }

            var eventSubData = await Util.GetMessageAs<EventSubData>(TwitchAPI.SubscribeToChannelChatMessage(
                client,
                broadcaster.Id,
                broadcaster.Id, // TODO: figure out why permission fails when this it bot.Id
                sessionId
            ));
            if (eventSubData is null) {
                return false;
            }

            webSocketClient.SetNotificationHandler("channel.chat.message", async (eventElement) => {
                ChannelChatMessageEvent eventStruct;
                try {
                    eventStruct = JsonSerializer.Deserialize<ChannelChatMessageEvent>(eventElement);
                } catch (Exception e) {
                    Logger.Warning($"Cannot handle channel chat message event because JsonSerializer.Deserialize failed: {e}.");
                    return;
                }

                await handler(eventStruct);
            });
            return true;
        }
    }
}
