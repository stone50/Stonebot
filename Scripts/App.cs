namespace StoneBot.Scripts {
    using Bot_Core.Models.EventSub;
    using Core_Interface.EventSub;
    using Godot;
    using StoneBot.Scripts.Bot_Core.App_Cache;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public App() => _ = Task.Run(Init);

        public async Task<bool> Init() {
            _ = await EventSub.RemoveBy();  // remove all event sub subscriptions
            _ = await ChannelChatMessage.Connect(HandleChatMessage);
            await AppCache.WebSocketClient.Refresh();
            _ = await EventSub.RemoveBy();  // remove all event sub subscriptions
            _ = await ChannelChatMessage.Connect(HandleChatMessage);
            return true;
        }

        private static async Task HandleChatMessage(ChannelChatMessageEvent messageEvent) => await Task.Run(() => GD.Print($"New chat from {messageEvent.ChatterUserName}: {messageEvent.Message.Text}"));
    }
}
