namespace Stonebot.Scripts.UI {
    using Command;
    using Godot;

    internal partial class TogglableCommandPanel : CommandPanel {
        public void Init(TogglableCommand command) {
            base.Init(command);
            Command = command;
            MainButton.Modulate = Command.IsEnabled ? Colors.White : Colors.Red;
            EnableButton.Icon = Command.IsEnabled ? Resources.EnableIcon : Resources.DisableIcon;
            EnableButton.Pressed += OnEnableButtonPressed;

            Command.IsEnabledChanged += OnIsEnabledChanged;

            EnableButton.MouseEntered += OnMouseEnteredEnableButton;
            EnableButton.MouseExited += OnMouseExitedEnableButton;
        }

        [Export]
        private Button EnableButton = null!;

        private new TogglableCommand Command = null!;

        private bool IsHovering = false;

        private void OnEnableButtonPressed() => Command.IsEnabled = !Command.IsEnabled;

        private void OnIsEnabledChanged(object? _, bool isEnabled) {
            MainButton.Modulate = isEnabled ? Colors.White : Colors.Red;
            UpdateIcon();
        }

        private void OnMouseEnteredEnableButton() {
            IsHovering = true;
            UpdateIcon();
        }

        private void OnMouseExitedEnableButton() {
            IsHovering = false;
            UpdateIcon();
        }

        private void UpdateIcon() => EnableButton.Icon = Command.IsEnabled ^ IsHovering ? Resources.EnableIcon : Resources.DisableIcon;
    }
}
