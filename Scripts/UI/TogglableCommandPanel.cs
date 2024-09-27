namespace StoneBot.Scripts.UI {
    using Command;
    using Godot;

    internal partial class TogglableCommandPanel : CommandPanel {
        [Export]
        private Texture2D EnableIcon = null!;
        [Export]
        private Texture2D DisableIcon = null!;
        [Export]
        private Button EnableButton = null!;

        private new TogglableCommand Command = null!;

        public void Init(TogglableCommand command) {
            base.Init(command);
            Command = command;
            MainButton.Modulate = Command.IsEnabled ? Colors.White : Colors.Red;
            EnableButton.Icon = Command.IsEnabled ? DisableIcon : EnableIcon;
            EnableButton.Pressed += OnEnableButtonPressed;

            Command.IsEnabledChanged += OnIsEnabledChanged;
        }

        private void OnEnableButtonPressed() => Command.IsEnabled = !Command.IsEnabled;

        private void OnIsEnabledChanged(object? _, bool isEnabled) {
            MainButton.Modulate = isEnabled ? Colors.White : Colors.Red;
            EnableButton.Icon = isEnabled ? DisableIcon : EnableIcon;
        }
    }
}
