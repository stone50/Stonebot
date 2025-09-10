namespace Stonebot.UI.Command {
    using CustomControls.Buttons;

    internal class PermissionLevelDropDownOption : InfoButton {
        public readonly UserPermission.Level PermissionLevel;

        public PermissionLevelDropDownOption(UserPermission.Level permissionLevel) {
            Content = permissionLevel;
            CornerRadius = new(0d);
            Margin = new(0d, 1d);

            PermissionLevel = permissionLevel;
        }
    }
}
