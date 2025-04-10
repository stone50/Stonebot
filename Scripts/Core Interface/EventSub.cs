namespace Stonebot.Scripts.Core_Interface.EventSub {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Bot_Core.Twitch;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class EventSub {
        // only up to 1 of status, type, and userId should be specified
        public static async Task<EventSubsData?> Get(string? status = null, string? type = null, string? userId = null) {
            Logger.Info("Getting event subs.");

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not get event subs because collector client wrapper get attempt failed.");
                return null;
            }

            EventSubsData? combinedData = null;
            string? cursor = null;
            while (true) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    Logger.Warning("Could not get event subs because client wrapper get client attempt failed.");
                    return null;
                }

                var potentialData = await Util.GetMessageAs<EventSubsData>(TwitchAPI.GetEventSubs(client, status, type, userId, cursor));
                if (potentialData is null) {
                    Logger.Warning("Could not get event subs because Twitch API get event subs attempt failed.");
                    return null;
                }

                var data = (EventSubsData)potentialData;
                if (combinedData is null) {
                    combinedData = data;
                } else {
                    var newCombinedData = (EventSubsData)combinedData;
                    newCombinedData.Data = [.. newCombinedData.Data, .. data.Data];
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
            Logger.Info("Removing event subs by filters.");

            var potentialData = await Get(status, type, userId);
            if (potentialData is null) {
                Logger.Warning("Could not remove event subs by filters because get attempt failed.");
                return false;
            }

            if (!await Remove(((EventSubsData)potentialData).Data)) {
                Logger.Warning("Could not remove event subs by filters because remove attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Remove(EventSubData[] eventSubs) {
            Logger.Info("Removing event subs.");

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not remove event subs because collector client wrapper get attempt failed.");
                return false;
            }

            foreach (var eventSub in eventSubs) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    Logger.Warning("Could not remove event subs because client wrapper get client attempt failed.");
                    return false;
                }

                if (!await Util.GetIsSuccess(TwitchAPI.DeleteEventSub(client, eventSub.Id))) {
                    Logger.Warning("Could not remove event subs because Twitch API delete event sub attempt failed.");
                    return false;
                }
            }

            return true;
        }

        public static async Task<bool> Add(EventSubData eventSub) {
            Logger.Info("Adding event sub.");

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not add event sub because collector client wrapper get attempt failed.");
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning("Could not add event sub because client wrapper get client attempt failed.");
                return false;
            }

            if (!await Util.GetIsSuccess(TwitchAPI.AddEventSub(client, eventSub))) {
                Logger.Warning("Could not add event sub because Twitch API add event sub attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> ConnectChannelChatMessage(Func<ChannelChatMessageEvent, Task> handler) {
            Logger.Info("Connecting to channel chat message event sub.");

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not connect to channel chat message event sub because config get attempt failed.");
                return false;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not connect to channel chat message event sub because broadcaster get attempt failed.");
                return false;
            }

            var bot = await AppCache.Bot.Get();
            if (bot is null) {
                Logger.Warning("Could not connect to channel chat message event sub because bot get attempt failed.");
                return false;
            }

            var webSocketClient = AppCache.WebSocketClient.Get();
            if (webSocketClient is null) {
                Logger.Warning("Could not connect to channel chat message event sub because web socket client get attempt failed.");
                return false;
            }

            var sessionId = await webSocketClient.GetId();
            if (sessionId is null) {
                Logger.Warning("Could not connect to channel chat message event sub because web socket client get id attempt failed.");
                return false;
            }

            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not connect to channel chat message event sub because chatter client wrapper get attempt failed.");
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning("Could not connect to channel chat message event sub because client wrapper get client attempt failed.");
                return false;
            }

            if (!await Util.GetIsSuccess(TwitchAPI.SubscribeToChannelChatMessage(
                client,
                broadcaster.Id,
                bot.Id,
                sessionId
            ))) {
                Logger.Warning("Could not connect to channel chat message event sub because Twitch API subscribe to channel chat message attempt failed.");
                return false;
            }

            webSocketClient.SetNotificationHandler("channel.chat.message", async (eventElement) => {
                ChannelChatMessageEvent eventStruct;
                try {
                    eventStruct = JsonSerializer.Deserialize<ChannelChatMessageEvent>(eventElement);
                } catch (Exception e) {
                    Logger.Warning($"Could not handle channel chat message event because json serializer deserialize attempt failed: {e}.");
                    return;
                }

                await handler(eventStruct);
            });
            return true;
        }
    }
}
