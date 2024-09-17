namespace StoneBot.Scripts {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Core_Interface.EventSub;
    using Godot;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public App() => _ = Task.Run(Init);

        private async Task<bool> Init() {
            _ = await AppCache.Load();
            _ = await EventSub.RemoveBy();  // remove all event sub subscriptions
            _ = await ChannelChatMessage.Connect(HandleChatMessage);
            return true;
        }

        private static async Task HandleChatMessage(ChannelChatMessageEvent messageEvent) => await Task.Run(() => GD.Print($"New chat from {messageEvent.ChatterUserName}: {messageEvent.Message.Text}"));

        public override void _Notification(int what) {
            if (what == NotificationWMCloseRequest) {
                Task.Run(async () => await AppCache.Save()).Wait();
                GetTree().Quit();
            }
        }
    }
}
