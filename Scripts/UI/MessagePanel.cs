namespace Stonebot.Scripts.UI {
    using Godot;
    using Message;
    using System;

    internal partial class MessagePanel : Control {
        public void Init(Message message) {
            Message = message;

            KeywordLabel.Text = Message.Keyword;
            MainButton.Pressed += OnMainButtonPressed;

            PermissionLevelMenuButton.Text = Message.PermissionLevel.ToString();
            foreach (var permissionLevel in Enum.GetValues<PermissionLevel>()) {
                PermissionLevelMenuButton.GetPopup().AddItem(permissionLevel.ToString(), (int)permissionLevel);
            }

            PermissionLevelMenuButton.GetPopup().IdPressed += OnPermissionLevelMenuButtonPopupIdPressed;
            UseDelaySpinBox.Value = Message.UseDelay;
            UseDelaySpinBox.ValueChanged += OnUseDelaySpinBoxValueChanged;

            Message.PermissionLevelChanged += OnPermissionLevelChanged;
            Message.UseDelayChanged += OnUseDelayChanged;

            MainButton.Modulate = Message.IsEnabled ? Colors.White : Colors.Red;
            EnableButton.Icon = Message.IsEnabled ? Resources.EnableIcon : Resources.DisableIcon;
            EnableButton.Pressed += OnEnableButtonPressed;

            Message.IsEnabledChanged += OnIsEnabledChanged;

            EnableButton.MouseEntered += OnMouseEnteredEnableButton;
            EnableButton.MouseExited += OnMouseExitedEnableButton;
        }

        [Export]
        private Label KeywordLabel = null!;
        [Export]
        private Button MainButton = null!;
        [Export]
        private Container DetailsContainer = null!;
        [Export]
        private MenuButton PermissionLevelMenuButton = null!;
        [Export]
        private SpinBox UseDelaySpinBox = null!;
        [Export]
        private Button EnableButton = null!;

        private Message Message = null!;

        private bool IsHovering = false;

        private void OnMainButtonPressed() => DetailsContainer.Visible = !DetailsContainer.Visible;

        private void OnPermissionLevelMenuButtonPopupIdPressed(long id) => Message.PermissionLevel = (PermissionLevel)id;

        private void OnUseDelaySpinBoxValueChanged(double value) => Message.UseDelay = (int)value;

        private void OnPermissionLevelChanged(object? _, PermissionLevel permissionLevel) => Util.CallDeferred(() => PermissionLevelMenuButton.Text = permissionLevel.ToString());

        private void OnUseDelayChanged(object? _, int useDelay) => Util.CallDeferred(() => UseDelaySpinBox.Value = useDelay);

        private void OnEnableButtonPressed() => Message.IsEnabled = !Message.IsEnabled;

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

        private void UpdateIcon() => EnableButton.Icon = Message.IsEnabled ^ IsHovering ? Resources.EnableIcon : Resources.DisableIcon;
    }
}
