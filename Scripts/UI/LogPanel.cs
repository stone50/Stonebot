namespace Stonebot.Scripts.UI {
    using Godot;

    internal partial class LogPanel : Control {
        public void Init(string logMessage) => MessageLabel.Text = logMessage;

        [Export]
        protected Label MessageLabel = null!;
    }
}
