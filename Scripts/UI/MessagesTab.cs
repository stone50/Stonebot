namespace StoneBot.Scripts.UI {
    using Godot;
    using Message;

    internal partial class MessagesTab : Control {
        [Export]
        private Container MessagesContainer = null!;

        public override void _Ready() {
            foreach (var message in MessageHandler.Messages) {
                var messagePanel = Resources.MessagePanelScene.Instantiate<MessagePanel>();
                messagePanel.Init(message);
                MessagesContainer.AddChild(messagePanel);
            }
        }
    }
}
