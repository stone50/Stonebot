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

            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            if (clientWrapper is null) {
                return false;
            }

            var client = await clientWrapper.GetClient();
            return client is not null && await Util.GetSuccessfulString(TwitchAPI.SendChatMessage(client, broadcaster.Id, bot.Id, message, replyParentMessageId)) is not null;
        }
    }
}
