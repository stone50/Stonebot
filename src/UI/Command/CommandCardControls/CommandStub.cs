namespace Stonebot.UI.Command.CommandCardControls {
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Avalonia.Media;
    using CustomControls;
    using CustomControls.Buttons;
    using Helpers;
    using Scripting;

    internal class CommandStub : Border {
        public readonly Command Command;

        public CommandStub(Command command, Swappable swappableCommandDisplay) {
            CornerRadius = new(5d);
            Margin = new(10d);
            Width = 400d;

            Command = command;
            var nameTextBlock = GetNameTextBlock();
            var enableToggleButton = GetEnableToggleButton();
            var nameRow = GetNameRow(nameTextBlock, enableToggleButton);
            var nameRowInnerBorder = GetNameRowInnerBorder(nameRow);
            var nameRowBorder = GetNameRowBorder(nameRowInnerBorder);
            enableToggleButton.Click += GetOnEnableToggleButtonClick(nameRowBorder, enableToggleButton);
            var expandButton = GetExpandButton(swappableCommandDisplay);
            Child = GetMainGrid(nameRowBorder, expandButton);
        }

        private static SGrid GetMainGrid(Border nameRowBorder, SButton expandButton) => new([
            GridLength.Auto,
            GridLength.Auto,
        ], [
            GridLength.Star,
        ], [
            nameRowBorder,
            expandButton,
        ]);

        private Border GetNameRowBorder(Border nameRowInnerBorder) => new() {
            Child = nameRowInnerBorder,
            BorderBrush = Command.Enabled ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2,
            BorderThickness = new(2d),
            CornerRadius = new(5d),
            Height = 70d,
            Width = 400d,
        };

        private static EventHandler<RoutedEventArgs> GetOnEnableToggleButtonClick(
            Border nameRowBorder,
            ToggleButton enableToggleButton
        ) => (_, _) => nameRowBorder.BorderBrush = enableToggleButton.State ? MainTheme.SuccessBrush2 : MainTheme.DangerBrush2;

        private static Border GetNameRowInnerBorder(SGrid nameRow) => new() {
            BorderBrush = MainTheme.PrimaryBrush2,
            BorderThickness = new(2d),
            Child = nameRow,
            CornerRadius = new(5d),
        };

        private static SGrid GetNameRow(SSelectableTextBlock nameTextBlock, ToggleButton enableToggleButton) => new([
            GridLength.Star,
        ], [
            GridLength.Star,
            GridLength.Auto,
        ], [
            nameTextBlock,
            enableToggleButton,
        ]) {
            Background = MainTheme.PrimaryBrush2,
        };

        private SSelectableTextBlock GetNameTextBlock() => new() {
            Text = $"!{Command.Name}",
            FontSize = 24d,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

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

        private static SButton GetExpandButton(Swappable swappableCommandDisplay) {
            var expandButton = new SButton(
                MainTheme.PrimaryBrush3,
                MainTheme.PrimaryBrush4,
                MainTheme.PrimaryBrush2
            ) {
                Content = "V",
                Margin = new(0d),
            };
            expandButton.Click += (_, _) => swappableCommandDisplay.Swap();
            return expandButton;
        }
    }
}
