namespace StoneBot.Scripts.UI {
    using Godot;
    using System;

    internal partial class InfoLog : Control {
        [Export]
        private Label MessageLabel = null!;

        public void Init(string message) => MessageLabel.Text = $"[{DateTime.Now}] INFO: {message}";
    }
}
