namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Layout;
    using Avalonia.Media;
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
            var nameEditButton = GetNameEditButton();
            var staticNameGroup = GetStaticNameGroup(nameTextBlock, nameEditButton);
            var nameTextBox = GetNameTextBox(command);
            var nameCancelButton = GetNameCancelButton();
            var nameSubmitButton = GetNameSubmitButton();
            var editableNameGroup = GetEditableNameGroup(nameTextBox, nameCancelButton, nameSubmitButton);
            var swappableName = new Swappable(staticNameGroup, editableNameGroup);
            nameEditButton.Click += (_, _) => {
                swappableName.Swap();
                _ = nameTextBox.Focus();
                nameTextBox.CaretIndex = nameTextBox.Text!.Length;
            };
            nameCancelButton.Click += (_, _) => {
                nameTextBox.Text = command.Name;
                swappableName.Swap();
            };
            nameSubmitButton.Click += (_, _) => OnNameSubmit(command, nameTextBlock, nameTextBox, swappableName);
            nameTextBox.KeyUp += (_, e) => {
                if (e.Key == Key.Enter) {
                    OnNameSubmit(command, nameTextBlock, nameTextBox, swappableName);
                }
            };
            var enableToggleButton = GetEnableToggleButton(command);
            var nameRow = GetNameRow(swappableName, enableToggleButton);
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
                    HorizontalAlignment = HorizontalAlignment.Left,
                },
                permissionRow,
                cooldownRow,
            ]) {
        };

        private static Border GetNameRow(Swappable swappableName, ToggleButton enableToggleButton) {
            var nameRow = new Border() {
                Child = new SGrid([
                    GridLength.Auto,
                ], [
                    GridLength.Star,
                    GridLength.Auto,
                ], [
                    swappableName,
                    enableToggleButton,
                ]) {
                    VerticalAlignment = VerticalAlignment.Center,
                },
                BorderBrush = enableToggleButton.State ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2,
                BorderThickness = new(2d),
                CornerRadius = new(5d),
                Height = 70d,
            };
            enableToggleButton.Click += (_, _) => nameRow.BorderBrush = enableToggleButton.State ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2;
            return nameRow;
        }

        private static STextBlock GetNameTextBlock(string name) => new() {
            Text = $"!{name}",
            FontSize = 24d,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        private static InfoButton GetNameEditButton() => new() {
            Content = UIUtils.GetPencilIcon(),
            Padding = new(5d),
            Height = 30d,
        };

        private static SGrid GetStaticNameGroup(STextBlock textBlock, InfoButton editButton) => new([
                GridLength.Auto,
            ], [
                GridLength.Auto,
                GridLength.Auto,
            ], [
                textBlock,
                editButton,
            ]);

        private static STextBox GetNameTextBox(Command command) {
            var nameTextBox = new STextBox() {
                Text = command.Name,
                FontSize = 24d,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 230d,
            };
            nameTextBox.TextChanging += (_, _) => {
                var newName = nameTextBox.Text!;
                if (newName == command.Name) {
                    nameTextBox.Background = null;
                    return;
                }

                if (!IsNewNameValid(newName)) {
                    nameTextBox.Background = MainTheme.DangerBrush1;
                    return;
                }

                nameTextBox.Background = null;
            };
            return nameTextBox;
        }

        private static DangerButton GetNameCancelButton() => new() {
            Content = UIUtils.GetCrossIcon(),
            Padding = new(7d),
            Margin = new(5d),
            Height = 30d,
        };

        private static SuccessButton GetNameSubmitButton() => new() {
            Content = UIUtils.GetCheckIcon(),
            Padding = new(7d),
            Margin = new(5d),
            Height = 30d,
        };

        private static SGrid GetEditableNameGroup(STextBox textBox, DangerButton cancelButton, SuccessButton submitButton) => new([
                GridLength.Auto,
            ], [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                textBox,
                cancelButton,
                submitButton,
            ]);

        private static void OnNameSubmit(
            Command command,
            STextBlock nameTextBlock,
            STextBox nameTextBox,
            Swappable swappableName
        ) {
            var newName = nameTextBox.Text!;
            if (newName == command.Name) {
                swappableName.Swap();
                return;
            }

            if (!IsNewNameValid(newName)) {
                return;
            }

            swappableName.Swap();
            nameTextBlock.Text = $"!{newName}";
            var oldScriptFilePath = command.GetScriptFilePath();
            command.Name = newName;
            _ = Utils.FireTryElseError(() => {
                File.Move(oldScriptFilePath, command.GetScriptFilePath());
                command.ReloadScriptFile();
                CommandManager.Save();
            }, CancellationToken.None);
        }

        private static ToggleButton GetEnableToggleButton(Command command) {
            var enableToggleButton = new ToggleButton(command.Enabled) {
                Content = UIUtils.GetPowerIcon(),
                VerticalAlignment = VerticalAlignment.Center,
                CornerRadius = new(20d),
                Width = 30d,
                Height = 30d,
                Padding = new(7d),
            };
            enableToggleButton.Click += (_, _) => {
                command.Enabled = !command.Enabled;
                _ = Utils.FireTryElseError(CommandManager.Save, CancellationToken.None);
            };
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
                _ = Utils.FireTryElseError(CommandManager.Save, CancellationToken.None);
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
            cooldownInput.ValueChanged += (_, _) => {
                command.CooldownSeconds = (int)cooldownInput.Value!;
                _ = Utils.FireTryElseError(CommandManager.Save, CancellationToken.None);
            };
            return cooldownInput;
        }

        private static bool IsNewNameValid(string newName) {
            if (newName.Length is 0 or > Constants.NumMaxCommandNameChars) {
                return false;
            }

            if (!newName.All(char.IsLetter)) {
                return false;
            }

            foreach (var command in CommandManager.Commands) {
                if (newName == command.Name) {
                    return false;
                }
            }

            return true;
        }
    }
}
