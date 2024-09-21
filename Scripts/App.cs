namespace StoneBot.Scripts {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Command;
    using Core_Interface;
    using Core_Interface.EventSub;
    using Godot;
    using Message;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public App() => _ = Task.Run(Init);

        private async Task<bool> Init() {
            _ = await AppCache.Init();
            _ = await EventSub.ConnectChannelChatMessage(HandleChatMessage);
            _ = await Chat.Send("MercyWing1 :) MercyWing2");
            return true;
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

        public override void _Notification(int what) {
            if (what == NotificationWMCloseRequest) {
                var logOffTask = Task.Run(async () => await Chat.Send("logging off..."));
                var saveCacheTask = Task.Run(async () => await AppCache.Save());
                var clearEventSubsTask = Task.Run(async () => await EventSub.RemoveBy());
                var closeWebSocketTask = Task.Run(async () => {
                    var webSocket = await AppCache.WebSocketClient.Get();
                    if (webSocket is null) {
                        return;
                    }

                    _ = await webSocket.Close();
                });
                Task.WaitAll(logOffTask, saveCacheTask, clearEventSubsTask, closeWebSocketTask);

                GetTree().Quit();
            }
        }
    }
}
