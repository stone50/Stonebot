namespace StoneBot.Scripts.UI {
    using Godot;
    using Message;

    internal partial class MessagesTab : Control {
        public override void _Ready() {
            foreach (var message in MessageHandler.Messages) {
                var messagePanel = Resources.MessagePanelScene.Instantiate<MessagePanel>();
                messagePanel.Init(message);
                MessagesContainer.AddChild(messagePanel);
            }
        }

        [Export]
        private Container MessagesContainer = null!;
    }
}
