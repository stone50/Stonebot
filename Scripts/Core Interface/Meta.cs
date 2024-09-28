namespace StoneBot.Scripts.Core_Interface {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Command;
    using Message;
    using System.Threading.Tasks;

    internal static class Meta {
        public static async Task Startup() {
            Logger.Info("Starting up.");
            _ = await AppCache.Init();
            _ = await EventSub.EventSub.ConnectChannelChatMessage(HandleChatMessage);
            _ = await Chat.Send("MercyWing1 :) MercyWing2");
        }

        public static async Task Shutdown() {
            Logger.Info("Shutting down.");
            _ = await Chat.Send("logging off...");
            await AppCache.Save();
            _ = await EventSub.EventSub.RemoveBy();
            var webSocket = await AppCache.WebSocketClient.Get();
            if (webSocket is not null) {
                _ = await webSocket.Close();
            }
        }

        private static async Task HandleChatMessage(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Handling chat message.");
            var bot = await AppCache.Bot.Get();
            if (bot is null) {
                return;
            }

            if (messageEvent.ChatterUserId == bot.Id) {
                return;
            }

            if (await CommandHandler.Handle(messageEvent)) {
                return;
            }

            if (await MessageHandler.Handle(messageEvent)) {
                return;
            }
        }
    }
}
