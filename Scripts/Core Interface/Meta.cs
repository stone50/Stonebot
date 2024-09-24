namespace StoneBot.Scripts.Core_Interface {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Message;
    using StoneBot.Scripts.Command;
    using StoneBot.Scripts.Timer;
    using System.Threading.Tasks;

    internal static class Meta {
        public static async Task Startup() {
            _ = await AppCache.Init();
            _ = await EventSub.EventSub.ConnectChannelChatMessage(HandleChatMessage);
            TimerManager.Init();
            _ = await Chat.Send("MercyWing1 :) MercyWing2 ");
        }

        public static async Task Shutdown() {
            _ = await Chat.Send("logging off...");
            _ = await AppCache.Save();
            _ = await EventSub.EventSub.RemoveBy();
            var webSocket = await AppCache.WebSocketClient.Get();
            if (webSocket is not null) {
                _ = await webSocket.Close();
            }
        }

        private static async Task HandleChatMessage(ChannelChatMessageEvent messageEvent) {
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
