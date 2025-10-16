namespace Stonebot.UI.Command.CommandCardControls {
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Avalonia.Media;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Popups;
    using Helpers;
    using Scripting;
    using System.Diagnostics;

    internal class CommandCard : Border {
        public readonly Command Command;

        public CommandCard(Command command, DeleteCommandPopup deleteCommandPopup) {
            Background = MainTheme.PrimaryBrush2;
            CornerRadius = new(5d);
            Margin = new(10d);
            Width = 400d;

            Command = command;
            var nameTextBlock = GetNameTextBlock();
            var nameEditButton = GetNameEditButton();
            var staticNameGroup = GetStaticNameGroup(nameTextBlock, nameEditButton);
            var nameTextBox = GetNameTextBox();
            var nameCancelButton = GetNameCancelButton();
            var nameSubmitButton = GetNameSubmitButton();
            var editableNameGroup = GetEditableNameGroup(nameTextBox, nameCancelButton, nameSubmitButton);
            var swappableName = new Swappable(staticNameGroup, editableNameGroup);
            nameEditButton.Click += GetOnNameEditButtonClick(swappableName, nameTextBox);
            nameCancelButton.Click += GetOnNameCancelButtonClick(swappableName, nameTextBox);
            nameSubmitButton.Click += GetOnNameSubmitButtonClick(swappableName, nameTextBlock, nameTextBox);
            nameTextBox.KeyUp += GetOnNameTextBoxKeyUp(swappableName, nameTextBlock, nameTextBox);
            var enableToggleButton = GetEnableToggleButton();
            var nameRow = GetNameRow(swappableName, enableToggleButton);
            var nameRowBorder = GetNameRowBorder(nameRow);
            enableToggleButton.Click += GetOnEnableToggleButtonClick(nameRowBorder, enableToggleButton);
            var permissionRowLabel = GetPermissionRowLabel();
            var permissionDropDownOptions = GetPermissionDropDownOptions();
            var permissionDropDown = GetPermissionDropDown(permissionDropDownOptions);
            var permissionInputFlyout = GetPermissionInputFlyout(permissionDropDown);
            var permissionInput = GetPermissionInput(permissionInputFlyout);
            foreach (var permissionDropDownOption in permissionDropDownOptions) {
                permissionDropDownOption.Click += GetOnPermissionDropDownOptionClick(permissionInput, permissionDropDownOption.PermissionLevel);
            }

            var permissionRow = GetPermissionRow(permissionRowLabel, permissionInput);
            var cooldownRowLabel = GetCooldownRowLabel();
            var cooldownInput = GetCooldownInput();
            var cooldownRow = GetCooldownRow(cooldownRowLabel, cooldownInput);
            var editScriptButton = GetEditScriptButton(command);
            var deleteButton = GetDeleteButton(command, deleteCommandPopup);
            var footer = GetFooter(editScriptButton, deleteButton);
            Child = GetMainGrid(nameRowBorder, permissionRow, cooldownRow, footer);
        }

        private static SGrid GetMainGrid(Border nameRow, SGrid permissionRow, SGrid cooldownRow, SGrid footer) => new([
            GridLength.Auto,
            //GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
        ], [
            GridLength.Star,
        ], [
            nameRow,
            // TODO
            //new SSelectableTextBlock(){
            //    Text = "Aliases",
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //},
            permissionRow,
            cooldownRow,
            footer,
        ]);

        private Border GetNameRowBorder(SGrid nameRow) => new() {
            Child = nameRow,
            BorderBrush = Command.Enabled ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2,
            BorderThickness = new(2d),
            CornerRadius = new(5d),
            Height = 70d,
        };

        private static EventHandler<RoutedEventArgs> GetOnEnableToggleButtonClick(
            Border nameRowBorder,
            ToggleButton enableToggleButton
        ) => (_, _) => nameRowBorder.BorderBrush = enableToggleButton.State ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2;

        private static SGrid GetNameRow(Swappable swappableName, ToggleButton enableToggleButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Star,
            GridLength.Auto,
        ], [
            swappableName,
            enableToggleButton,
        ]) {
            VerticalAlignment = VerticalAlignment.Center,
        };

        private SSelectableTextBlock GetNameTextBlock() => new() {
            Text = $"!{Command.Name}",
            FontSize = 24d,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        private static InfoButton GetNameEditButton() => new() {
            Content = UIUtils.GetPencilIcon(),
            Padding = new(5d),
            Height = 30d,
        };

        private static SGrid GetStaticNameGroup(SSelectableTextBlock textBlock, InfoButton editButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            textBlock,
            editButton,
        ]);

        private STextBox GetNameTextBox() {
            var nameTextBox = new STextBox() {
                Text = Command.Name,
                FontSize = 24d,
                FontWeight = FontWeight.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 230d,
            };
            nameTextBox.TextChanging += (_, _) => {
                var newName = nameTextBox.Text!;
                if (newName == Command.Name) {
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

        private static EventHandler<RoutedEventArgs> GetOnNameEditButtonClick(Swappable swappableName, STextBox nameTextBox) => (_, _) => {
            swappableName.Swap();
            _ = nameTextBox.Focus();
            nameTextBox.CaretIndex = nameTextBox.Text!.Length;
        };

        private EventHandler<RoutedEventArgs> GetOnNameCancelButtonClick(Swappable swappableName, STextBox nameTextBox) => (_, _) => {
            nameTextBox.Text = Command.Name;
            swappableName.Swap();
        };

        private EventHandler<RoutedEventArgs> GetOnNameSubmitButtonClick(
            Swappable swappableName,
            SSelectableTextBlock nameTextBlock,
            STextBox nameTextBox
        ) => (_, _) => OnNameSubmit(swappableName, nameTextBlock, nameTextBox);

        private EventHandler<KeyEventArgs> GetOnNameTextBoxKeyUp(
            Swappable swappableName,
            SSelectableTextBlock nameTextBlock,
            STextBox nameTextBox
        ) => (_, e) => {
            if (e.Key == Key.Enter) {
                OnNameSubmit(swappableName, nameTextBlock, nameTextBox);
            }
        };

        private void OnNameSubmit(
            Swappable swappableName,
            SSelectableTextBlock nameTextBlock,
            STextBox nameTextBox
        ) {
            var newName = nameTextBox.Text!;
            if (newName == Command.Name) {
                swappableName.Swap();
                return;
            }

            if (!IsNewNameValid(newName)) {
                return;
            }

            swappableName.Swap();
            nameTextBlock.Text = $"!{newName}";
            var oldScriptFilePath = Command.GetScriptFilePath();
            Command.Name = newName;
            try {
                File.Move(oldScriptFilePath, Command.GetScriptFilePath());
                Command.ReloadScriptFile();
                CommandManager.Save();
            } catch (Exception e) {
                Logger.Error(e);
            }

            InteractionGrid.Update();
        }

        private ToggleButton GetEnableToggleButton() {
            var enableToggleButton = new ToggleButton(Command.Enabled) {
                Content = UIUtils.GetPowerIcon(),
                VerticalAlignment = VerticalAlignment.Center,
                CornerRadius = new(20d),
                Width = 30d,
                Height = 30d,
                Padding = new(7d),
            };
            enableToggleButton.Click += (_, _) => {
                Command.Enabled = !Command.Enabled;
                _ = TaskHelper.FireTryElseError(CommandManager.Save);
            };
            return enableToggleButton;
        }

        private static SGrid GetPermissionRow(SSelectableTextBlock permissionRowLabel, InfoButton permissionInput) => new([
                GridLength.Auto,
            ], [
                GridLength.Auto,
                GridLength.Auto,
            ], [
                permissionRowLabel,
                permissionInput,
            ]);

        private static SSelectableTextBlock GetPermissionRowLabel() => new() {
            Text = "Permission Level",
        };

        private InfoButton GetPermissionInput(Flyout permissionInputFlyout) => new() {
            Content = $"{Command.PermissionLevel} ▼",
            Flyout = permissionInputFlyout,
        };

        private static Flyout GetPermissionInputFlyout(SGrid permissionDropDownOptions) => new() {
            Content = permissionDropDownOptions,
        };

        private static SGrid GetPermissionDropDown(InfoButton[] permissionDropDownOptions) => new(
            [.. Enumerable.Repeat(GridLength.Auto, permissionDropDownOptions.Length)],
            [GridLength.Auto],
            [.. permissionDropDownOptions]
        );

        private static PermissionLevelDropDownOption[] GetPermissionDropDownOptions() => [.. Enum.GetValues<UserPermission.Level>().Reverse().Select(permissionLevel => new PermissionLevelDropDownOption(permissionLevel))];

        private EventHandler<RoutedEventArgs> GetOnPermissionDropDownOptionClick(InfoButton permissionInput, UserPermission.Level permissionLevel) => (_, _) => {
            Command.PermissionLevel = permissionLevel;
            permissionInput.Content = $"{Command.PermissionLevel} ▼";
            permissionInput.Flyout!.Hide();
            _ = TaskHelper.FireTryElseError(CommandManager.Save);
        };

        private static SGrid GetCooldownRow(SSelectableTextBlock cooldownRowLabel, NumericUpDown cooldownInput) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            cooldownRowLabel,
            cooldownInput,
        ]);

        private static SSelectableTextBlock GetCooldownRowLabel() => new() {
            Text = "Cooldown Seconds",
        };

        private SNumericUpDown GetCooldownInput() {
            var cooldownInput = new SNumericUpDown(0M, Constants.CommandCooldownSecsMax, true) {
                Value = Command.CooldownSeconds,
                Width = 75d,
            };
            cooldownInput.ValueChanged += (_, _) => {
                Command.CooldownSeconds = (int)cooldownInput.Value!;
                _ = TaskHelper.FireTryElseError(CommandManager.Save);
            };
            return cooldownInput;
        }

        private static SGrid GetFooter(InfoButton editScriptButton, DangerButton deleteButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            GridLength.Star,
        ], [
            editScriptButton,
            deleteButton,
        ]);

        private static InfoButton GetEditScriptButton(Command command) {
            var editScriptButton = new InfoButton() {
                Content = "Edit Script",
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            editScriptButton.Click += (_, _) => {
                try {
                    _ = Process.Start(new ProcessStartInfo {
                        FileName = command.GetScriptFilePath(),
                        UseShellExecute = true,
                    });
                } catch (Exception e) {
                    Logger.Error(e);
                }
            };
            return editScriptButton;
        }

        private static DangerButton GetDeleteButton(Command command, DeleteCommandPopup deleteCommandPopup) {
            var deleteButton = new DangerButton() {
                Content = UIUtils.GetTrashIcon(),
                Height = 32d,
                HorizontalAlignment = HorizontalAlignment.Right,
                Padding = new(7d),
                Width = 32d,
            };
            deleteButton.Click += (_, _) => deleteCommandPopup.Show(command);
            return deleteButton;
        }

        private static bool IsNewNameValid(string newName) {
            if (newName.Length is 0 or > Constants.NumMaxCommandNameChars) {
                return false;
            }

            if (!newName.All(c => char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))) {
                return false;
            }

            foreach (var command in CommandManager.Commands) {
                if (newName == command.Name) {
                    return false;
                }

                if (command.Aliases.Contains(newName)) {
                    return false;
                }
            }

            return true;
        }
    }
}
