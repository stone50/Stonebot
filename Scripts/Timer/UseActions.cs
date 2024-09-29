namespace Stonebot.Scripts.Timer {
    using Bot_Core.App_Cache;
    using Core_Interface;
    using System;
    using System.Threading.Tasks;

    internal static class UseActions {
        public static async Task Quote() {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            if (customData.Quotes.Count == 0) {
                return;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                return;
            }

            var quoteIndex = new Random().Next(customData.Quotes.Count);
            _ = await Chat.Send($"[{quoteIndex}] \"{customData.Quotes[quoteIndex]}\" -{broadcaster.UserName}");
        }
    }
}
