namespace Stonebot.Scripts.UI {
    using Command;
    using Godot;
    using System;

    internal partial class CommandPanel : Control {
        public void Init(Command command) {
            Command = command;

            KeywordLabel.Text = Command.Keyword;
            MainButton.Pressed += OnMainButtonPressed;

            PermissionLevelMenuButton.Text = Command.PermissionLevel.ToString();
            foreach (var permissionLevel in Enum.GetValues<PermissionLevel>()) {
                PermissionLevelMenuButton.GetPopup().AddItem(permissionLevel.ToString(), (int)permissionLevel);
            }

            PermissionLevelMenuButton.GetPopup().IdPressed += OnPermissionLevelMenuButtonPopupIdPressed;
            UseDelaySpinBox.Value = Command.UseDelay;
            UseDelaySpinBox.ValueChanged += OnUseDelaySpinBoxValueChanged;

            Command.PermissionLevelChanged += OnPermissionLevelChanged;
            Command.UseDelayChanged += OnUseDelayChanged;
        }

        [Export]
        protected Label KeywordLabel = null!;
        [Export]
        protected Button MainButton = null!;
        [Export]
        protected Container DetailsContainer = null!;
        [Export]
        protected MenuButton PermissionLevelMenuButton = null!;
        [Export]
        protected SpinBox UseDelaySpinBox = null!;

        protected Command Command = null!;

        private void OnMainButtonPressed() => DetailsContainer.Visible = !DetailsContainer.Visible;

        private void OnPermissionLevelMenuButtonPopupIdPressed(long id) => Command.PermissionLevel = (PermissionLevel)id;

        private void OnUseDelaySpinBoxValueChanged(double value) => Command.UseDelay = (int)value;

        private void OnPermissionLevelChanged(object? _, PermissionLevel permissionLevel) => Util.CallDeferred(() => PermissionLevelMenuButton.Text = permissionLevel.ToString());

        private void OnUseDelayChanged(object? _, int useDelay) => Util.CallDeferred(() => UseDelaySpinBox.Value = useDelay);
    }
}
