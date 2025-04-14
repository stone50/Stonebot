namespace Stonebot.Scripts.Core_Interface {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Twitch;
    using System.Threading.Tasks;

    internal static class Chat {
        public static async Task<bool> Send(string message, string? replyParentMessageId = null) {
            var logPrefix = $"{nameof(Chat)} | {nameof(Send)}";
            Logger.Info($"{logPrefix}\n{nameof(message)}: {message}\n{nameof(replyParentMessageId)}: {replyParentMessageId}");

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

            if (!await Util.GetIsSuccess(TwitchAPI.SendChatMessage(client, broadcaster.Id, bot.Id, message, replyParentMessageId))) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.SendChatMessage)} was unsuccessful.");
                return false;
            }

            return true;
        }
    }
}
