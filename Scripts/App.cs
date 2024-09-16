namespace StoneBot.Scripts {
    using Bot_Core.Models.EventSub;
    using Core_Interface.EventSub;
    using Godot;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public App() => _ = Task.Run(Init);

        public async Task<bool> Init() {
            _ = await EventSub.RemoveBy();  // remove all event sub subscriptions
            _ = await ChannelChatMessage.Connect(HandleChatMessage);
            return true;
        }

        private static async Task HandleChatMessage(ChannelChatMessageEvent messageEvent) => GD.Print($"New chat from {messageEvent.ChatterUserName}: {messageEvent.Message.Text}");
    }
}
