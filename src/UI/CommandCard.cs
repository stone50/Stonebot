namespace Stonebot.UI {
    using Avalonia.Controls;
    using Buttons;
    using Scripting;

    internal class CommandCard : Border {
        public readonly Command Command;

        public CommandCard(Command command) {
            Command = command;

            CornerRadius = new(5d);
            Background = MainTheme.PrimaryBrush2;
            Margin = new(10d);
            Width = 400d;

            var nameTextBlock = GetNameTextBlock(command.Name);
            var enableToggleButton = GetEnableToggleButton(command);
            var nameRow = GetNameRow(nameTextBlock, enableToggleButton);
            var permissionInput = GetPermissionInput(command);
            var permissionRow = GetPermissionRow(permissionInput);
            var cooldownInput = GetCooldownInput(command);
            var cooldownRow = GetCooldownRow(cooldownInput);
            Child = GetMainGrid(nameRow, permissionRow, cooldownRow);
        }

        private static SGrid GetMainGrid(Border nameRow, SGrid permissionRow, SGrid cooldownRow) => new([
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GridLength.Star,
            ], [
                nameRow,
                // TODO
                new STextBlock(){
                    Text = "Aliases",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                },
                permissionRow,
                cooldownRow,
            ]);

        private static Border GetNameRow(STextBlock nameTextBlock, ToggleButton enableToggleButton) {
            var nameRow = new Border() {
                Child = new SGrid([
                GridLength.Auto,
            ], [
                GridLength.Star,
                GridLength.Auto,
            ], [
                nameTextBlock,
                enableToggleButton,
            ]),
                BorderBrush = enableToggleButton.State ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2,
                BorderThickness = new(2d),
                CornerRadius = new(5d),
            };
            enableToggleButton.Click += (_, _) => nameRow.BorderBrush = enableToggleButton.State ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2;
            return nameRow;
        }

        private static STextBlock GetNameTextBlock(string name) {
            var displayName = Utils.GetCutoffText(name, Constants.NumMaxCommandNameChars);
            return new STextBlock() {
                Text = $"!{displayName}",
                FontSize = 24d,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            };
        }

        private static ToggleButton GetEnableToggleButton(Command command) {
            var enableToggleButton = new ToggleButton(command.Enabled) {
                Content = UIUtils.GetPowerIcon(),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                CornerRadius = new(20d),
                Width = 30d,
                Height = 30d,
                Padding = new(7d),
            };
            enableToggleButton.Click += (_, _) => command.Enabled = !command.Enabled;
            return enableToggleButton;
        }

        private static SGrid GetPermissionRow(InfoButton permissionInput) => new([
                GridLength.Auto,
            ], [
                GridLength.Auto,
                GridLength.Auto,
            ], [
                new STextBlock() {
                    Text = "Permission Level",
                },
                permissionInput,
            ]);

        private static InfoButton GetPermissionInput(Command command) {
            var permissionInput = new InfoButton() {
                Content = $"{command.PermissionLevel} ▼",
            };
            permissionInput.Flyout = new Flyout() {
                Content = GetPermissionDropDownOptions(command, permissionInput),
            };
            return permissionInput;
        }

        private static SGrid GetPermissionDropDownOptions(Command command, InfoButton permissionInput) {
            var permissionDropDownOptions = Enum.GetValues<UserPermission.Level>().Reverse().Select(permissionLevel => GetPermissionDropDownOption(command, permissionInput, permissionLevel));
            return new(
                [.. Enumerable.Repeat(GridLength.Auto, permissionDropDownOptions.Count())],
                [GridLength.Auto],
                [.. permissionDropDownOptions]
            );
        }

        private static InfoButton GetPermissionDropDownOption(Command command, InfoButton permissionInput, UserPermission.Level permissionLevel) {
            var dropDownOption = new InfoButton() {
                Content = permissionLevel,
                CornerRadius = new(0d),
                Margin = new(0d, 1d),
            };
            dropDownOption.Click += (_, _) => {
                command.PermissionLevel = permissionLevel;
                permissionInput.Content = $"{command.PermissionLevel} ▼";
                permissionInput.Flyout!.Hide();
            };
            return dropDownOption;
        }

        private static SGrid GetCooldownRow(NumericUpDown cooldownInput) => new([
                GridLength.Auto,
            ], [
                GridLength.Auto,
                GridLength.Auto,
            ], [
                 new STextBlock() {
                    Text = "Cooldown Seconds",
                },
                cooldownInput,
            ]);

        private static SNumericUpDown GetCooldownInput(Command command) {
            var cooldownInput = new SNumericUpDown(0, Constants.CommandCooldownSecondsMax, true) {
                Value = command.CooldownSeconds,
                Width = 75d,
            };
            cooldownInput.ValueChanged += (_, _) => command.CooldownSeconds = (int)cooldownInput.Value!;
            return cooldownInput;
        }
    }
}
