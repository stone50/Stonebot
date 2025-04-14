namespace Stonebot.Scripts.Core_Interface {
    using Bot_Core.App_Cache;
    using System.Threading.Tasks;

    internal static class Meta {
        public static async void Startup() {
            var logPrefix = $"{nameof(Meta)} | {nameof(Startup)}";
            Logger.Info(logPrefix);

            if (!await AppCache.Init()) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Init)} result is false.");
                return;
            }

            _ = await EventSub.EventSub.ConnectChannelChatMessage();
            _ = await Chat.Send("MercyWing1 :) MercyWing2");
        }

        public static async Task Shutdown() {
            Logger.Info($"{nameof(Meta)} | {nameof(Shutdown)}");

            if (AppCache.CollectorClientWrapper.GetWithoutRefresh() is null || AppCache.ChatterClientWrapper.GetWithoutRefresh() is null) {
                return;
            }

            _ = await EventSub.EventSub.RemoveBy();
            var webSocket = AppCache.WebSocketClient.Get();
            if (webSocket is not null) {
                _ = await webSocket.Close();
            }

            _ = await AppCache.SaveAll();
            _ = await Chat.Send("logging off...");
        }
    }
}
