namespace Stonebot.Scripts.Timer {
    using Bot_Core.App_Cache;
    using Core_Interface;
    using System;
    using System.Threading.Tasks;

    internal static class UseActions {
        public static async Task<bool> Quote() {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Quote)}";
            Logger.Info(logPrefix);

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
                return false;
            }

            if (customData.Quotes.Count == 0) {
                return true;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
                return false;
            }

            var quoteIndex = new Random().Next(customData.Quotes.Count);
            if (!await Chat.Send($"[{quoteIndex}] \"{customData.Quotes[quoteIndex]}\" -{broadcaster.UserName}")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }
    }
}
