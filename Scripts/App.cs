namespace StoneBot.Scripts {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Command;
    using Core_Interface;
    using Core_Interface.EventSub;
    using Godot;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public App() => _ = Task.Run(Init);

        private async Task<bool> Init() {
            _ = await AppCache.Load();
            _ = await EventSub.RemoveBy();  // remove all event sub subscriptions
            _ = await EventSub.ConnectChannelChatMessage(HandleChatMessage);
            _ = await Chat.Send("Hello World!");
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

            // TODO: add response handler
        }

        public override void _Notification(int what) {
            if (what == NotificationWMCloseRequest) {
                Task.Run(async () => await AppCache.Save()).Wait();
                GetTree().Quit();
            }
        }
    }
}
