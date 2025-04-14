namespace Stonebot.Scripts.Core_Interface.EventSub {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Bot_Core.Twitch;
    using Command;
    using Message;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class EventSub {
        // only up to 1 of status, type, and userId should be specified
        public static async Task<EventSubsData?> Get(string? status = null, string? type = null, string? userId = null) {
            var logPrefix = $"{nameof(EventSub)} | {nameof(Get)}";
            Logger.Info($"{logPrefix}\n{nameof(status)}: {status}\n{nameof(type)}: {type}\n{nameof(userId)}: {userId}");

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.CollectorClientWrapper.Get)} result is null.");
                return null;
            }

            EventSubsData? combinedData = null;
            string? cursor = null;
            while (true) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                    return null;
                }

                var potentialData = await Util.GetMessageAs<EventSubsData>(TwitchAPI.GetEventSubs(client, status, type, userId, cursor));
                if (potentialData is null) {
                    Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.GetEventSubs)} was unsuccessful.\n{nameof(cursor)}: {cursor}");
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
            var logPrefix = $"{nameof(EventSub)} | {nameof(RemoveBy)}";
            Logger.Info($"{logPrefix}\n{nameof(status)}: {status}\n{nameof(type)}: {type}\n{nameof(userId)}: {userId}");

            var potentialData = await Get(status, type, userId);
            if (potentialData is null) {
                Logger.Warning($"{logPrefix} | {nameof(Get)} result is null.");
                return false;
            }

            if (!await Remove(((EventSubsData)potentialData).Data)) {
                Logger.Warning($"{logPrefix} | {nameof(Remove)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Remove(EventSubData[] eventSubs) {
            var logPrefix = $"{nameof(EventSub)} | {nameof(Remove)}";
            Logger.Info($"{logPrefix}\n{nameof(eventSubs)}: {JsonSerializer.Serialize(eventSubs)}");

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.CollectorClientWrapper.Get)} result is null.");
                return false;
            }

            foreach (var eventSub in eventSubs) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                    return false;
                }

                if (!await Util.GetIsSuccess(TwitchAPI.DeleteEventSub(client, eventSub.Id))) {
                    Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.DeleteEventSub)} was unsuccessful.");
                    return false;
                }
            }

            return true;
        }

        public static async Task<bool> Add(EventSubData eventSub) {
            var logPrefix = $"{nameof(EventSub)} | {nameof(Add)}";
            Logger.Info($"{logPrefix}\n{nameof(eventSub)}: {JsonSerializer.Serialize(eventSub)}");

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.CollectorClientWrapper.Get)} result is null.");
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                return false;
            }

            if (!await Util.GetIsSuccess(TwitchAPI.AddEventSub(client, eventSub))) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.AddEventSub)} was unsuccessful.");
                return false;
            }

            return true;
        }

        public static async Task<bool> ConnectChannelChatMessage() {
            var logPrefix = $"{nameof(EventSub)} | {nameof(ConnectChannelChatMessage)}";
            Logger.Info(logPrefix);

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return false;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
                return false;
            }

            var bot = await AppCache.Bot.Get();
            if (bot is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Bot.Get)} result is null.");
                return false;
            }

            var webSocketClient = AppCache.WebSocketClient.Get();
            if (webSocketClient is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.WebSocketClient.Get)} result is null.");
                return false;
            }

            var sessionId = await webSocketClient.GetId();
            if (sessionId is null) {
                Logger.Warning($"{logPrefix} | {nameof(webSocketClient.GetId)} result is null.");
                return false;
            }

            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.ChatterClientWrapper.Get)} result is null.");
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                return false;
            }

            if (!await Util.GetIsSuccess(TwitchAPI.SubscribeToChannelChatMessage(
                client,
                broadcaster.Id,
                bot.Id,
                sessionId
            ))) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.SubscribeToChannelChatMessage)} was unsuccessful.");
                return false;
            }

            webSocketClient.SetNotificationHandler("channel.chat.message", ChannelChatMessageNotificationHandler);
            return true;
        }

        private static async Task ChannelChatMessageNotificationHandler(JsonElement eventElement) {
            var logPrefix = $"{nameof(EventSub)} | ChannelChatMessageNotificationHandler";
            Logger.Info($"{logPrefix}\n{nameof(eventElement)}: {eventElement}");

            ChannelChatMessageEvent eventStruct;
            try {
                eventStruct = JsonSerializer.Deserialize<ChannelChatMessageEvent>(eventElement);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(JsonSerializer.Deserialize)} threw: {e}.");
                return;
            }

            var bot = await AppCache.Bot.Get();
            if (bot is null) {
                Logger.Warning("Could not handle chat message because bot get attempt failed.");
                return;
            }

            if (eventStruct.ChatterUserId == bot.Id) {
                return;
            }

            var isCommandHandled = await CommandHandler.Handle(eventStruct);
            if (isCommandHandled is null) {
                Logger.Warning("Could not handle chat message because command handler handle attempt failed.");
                return;
            }

            if ((bool)isCommandHandled) {
                return;
            }

            var isMessageHandled = await MessageHandler.Handle(eventStruct);
            if (isMessageHandled is null) {
                Logger.Warning("Could not handle chat message because message handler handle attempt failed.");
                return;
            }

            if ((bool)isMessageHandled) {
                return;
            }
        }
    }
}
