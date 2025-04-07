namespace Stonebot.Scripts.Core_Interface {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Twitch;
    using System.Threading.Tasks;

    internal static class Chat {
        public static async Task<bool> Send(string message, string? replyParentMessageId = null) {
            Logger.Info("Sending a chat message.");

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not send chat message because config get attempt failed.");
                return false;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not send chat message because broadcaster get attempt failed.");
                return false;
            }

            var bot = await AppCache.Bot.Get();
            if (bot is null) {
                Logger.Warning("Could not send chat message because bot get attempt failed.");
                return false;
            }

            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not send chat message because chatter client wrapper get attempt failed.");
                return false;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning("Could not send chat message because client wrapper get client attempt failed.");
                return false;
            }

            if (!await Util.GetIsSuccess(TwitchAPI.SendChatMessage(client, broadcaster.Id, bot.Id, message, replyParentMessageId))) {
                Logger.Warning("Could not send chat message because Twitch API send chat message attempt failed.");
                return false;
            }

            return true;
        }
    }
}
