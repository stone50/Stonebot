namespace Stonebot.Scripts.UI {
    using Command;
    using Godot;

    internal partial class TogglableCommandPanel : CommandPanel {
        public void Init(TogglableCommand command) {
            base.Init(command);
            Command = command;
            MainButton.Modulate = Command.IsEnabled ? Colors.White : Colors.Red;
            EnableButton.Icon = Command.IsEnabled ? Resources.DisableIcon : Resources.EnableIcon;
            EnableButton.Pressed += OnEnableButtonPressed;

            Command.IsEnabledChanged += OnIsEnabledChanged;
        }

        [Export]
        private Button EnableButton = null!;

        private new TogglableCommand Command = null!;

        private void OnEnableButtonPressed() => Command.IsEnabled = !Command.IsEnabled;

        private void OnIsEnabledChanged(object? _, bool isEnabled) {
            MainButton.Modulate = isEnabled ? Colors.White : Colors.Red;
            EnableButton.Icon = isEnabled ? Resources.DisableIcon : Resources.EnableIcon;
        }
    }
}
