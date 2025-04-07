namespace Stonebot.Scripts.Timer {
    using Bot_Core.App_Cache;
    using Core_Interface;
    using System;
    using System.Threading.Tasks;

    internal static class UseActions {
        public static async Task<bool> Quote() {
            Logger.Info("Proccing quote action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not proc quote action because data get attempt failed.");
                return false;
            }

            if (customData.Quotes.Count == 0) {
                return true;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not proc quote action because broadcaster get attempt failed.");
                return false;
            }

            var quoteIndex = new Random().Next(customData.Quotes.Count);
            if (!await Chat.Send($"[{quoteIndex}] \"{customData.Quotes[quoteIndex]}\" -{broadcaster.UserName}")) {
                Logger.Warning("Could not proc quote action because chat send attempt failed.");
                return false;
            }

            return true;
        }
    }
}
