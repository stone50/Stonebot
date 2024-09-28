namespace StoneBot.Scripts.UI {
    using Godot;

    internal partial class LogPanel : Control {
        [Export]
        protected Label MessageLabel = null!;

        public void Init(string logMessage) => MessageLabel.Text = logMessage;
    }
}
