namespace Stonebot.UI.Command.CommandCardControls {
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Popups;
    using Scripting;

    internal class NewCommandPopup : SPopup {
        public NewCommandPopup(InteractionGrid interactionGrid) : base("New Command") {
            var nameLabel = GetNameLabel();
            nameTextBox = GetNameTextBox();
            var nameRow = GetNameRow(nameLabel, nameTextBox);
            var permissionRowLabel = GetPermissionRowLabel();
            var permissionDropDownOptions = GetPermissionDropDownOptions();
            var permissionDropDown = GetPermissionDropDown(permissionDropDownOptions);
            var permissionInputFlyout = GetPermissionInputFlyout(permissionDropDown);
            permissionInput = GetPermissionInput(permissionInputFlyout);
            foreach (var permissionDropDownOption in permissionDropDownOptions) {
                permissionDropDownOption.Click += GetOnPermissionDropDownOptionClick(permissionInput, permissionDropDownOption.PermissionLevel);
            }

            var permissionRow = GetPermissionRow(permissionRowLabel, permissionInput);
            var cooldownRowLabel = GetCooldownRowLabel();
            cooldownInput = GetCooldownInput();
            var cooldownRow = GetCooldownRow(cooldownRowLabel, cooldownInput);
            var mainGrid = GetMainGrid(nameRow, permissionRow, cooldownRow);
            Body.Children.Add(mainGrid);
            var cancelButton = GetCancelButton();
            var createButton = GetCreateButton(interactionGrid);
            var footerButtons = GetFooterButtons(cancelButton, createButton);
            Footer.Children.Add(footerButtons);
        }

        public void Show() {
            selectedPermissionLevel = UserPermission.Level.Broadcaster;
            permissionInput.Content = $"{selectedPermissionLevel} ▼";
            nameTextBox.Text = "example";
            cooldownInput.Value = 0M;
            IsVisible = true;
        }

        private UserPermission.Level selectedPermissionLevel;
        private readonly STextBox nameTextBox;
        private readonly InfoButton permissionInput;
        private readonly SNumericUpDown cooldownInput;

        private static SGrid GetMainGrid(SGrid nameRow, SGrid permissionRow, SGrid cooldownRow) => new([
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
            Width = 450d,
        };

        private static SGrid GetNameRow(STextBlock nameLabel, STextBox nameTextBox) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            nameLabel,
            nameTextBox,
        ]);

        private static STextBlock GetNameLabel() => new() {
            Margin = new(10d, 10d, 0d, 10d),
            Text = "Name !",
        };

        private static STextBox GetNameTextBox() {
            var nameTextBox = new STextBox() {
                Background = IsNewNameValid("example") ? null : MainTheme.DangerBrush1,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new(0d, 10d, 10d, 10d),
                MaxWidth = 300d,
            };
            nameTextBox.TextChanging += (_, _) => {
                var newName = nameTextBox.Text!;
                if (!IsNewNameValid(newName)) {
                    nameTextBox.Background = MainTheme.DangerBrush1;
                    return;
                }

                nameTextBox.Background = null;
            };
            return nameTextBox;
        }

        private static SGrid GetPermissionRow(STextBlock permissionRowLabel, InfoButton permissionInput) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            permissionRowLabel,
            permissionInput,
        ]);

        private static STextBlock GetPermissionRowLabel() => new() {
            Text = "Permission Level",
        };

        private static InfoButton GetPermissionInput(Flyout permissionInputFlyout) => new() {
            Content = $"{UserPermission.Level.Broadcaster} ▼",
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
            selectedPermissionLevel = permissionLevel;
            permissionInput.Content = $"{selectedPermissionLevel} ▼";
            permissionInput.Flyout!.Hide();
        };

        private static SGrid GetCooldownRow(STextBlock cooldownRowLabel, NumericUpDown cooldownInput) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            cooldownRowLabel,
            cooldownInput,
        ]);

        private static STextBlock GetCooldownRowLabel() => new() {
            Text = "Cooldown Seconds",
        };

        private static SNumericUpDown GetCooldownInput() => new(0M, Constants.CommandCooldownSecsMax, true) {
            Value = 0M,
            Width = 75d,
        };

        private static SGrid GetFooterButtons(DangerButton cancelButton, SuccessButton createButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Star,
            GridLength.Star,
        ], [
            cancelButton,
            createButton,
        ]) {
            Margin = new(0d, 10d, 0d, 0d),
        };

        private DangerButton GetCancelButton() {
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                Margin = new(0d, 0d, 10d, 0d),
                MaxHeight = 50d,
            };
            cancelButton.Click += (_, _) => IsVisible = false;
            return cancelButton;
        }

        private SuccessButton GetCreateButton(InteractionGrid interactionGrid) {
            var okButton = new SuccessButton() {
                Content = "Create",
                Margin = new(10d, 0d, 0d, 0d),
                MaxHeight = 50d,
            };
            okButton.Click += GetOnCreateClick(interactionGrid);
            return okButton;
        }

        private EventHandler<RoutedEventArgs> GetOnCreateClick(InteractionGrid interactionGrid) => (_, _) => {
            if (!IsNewNameValid(nameTextBox.Text!)) {
                return;
            }

            var newCommand = new Command(
                nameTextBox.Text!,
                [], // TODO
                false,
                selectedPermissionLevel,
                (int)cooldownInput.Value!
            );
            CommandManager.Commands.Add(newCommand);
            try {
                CommandManager.Save();
            } catch (Exception e) {
                Logger.Error(e);
            }
            interactionGrid.Update();
            IsVisible = false;
        };

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
