namespace Stonebot.UI.CustomControls.Buttons {
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Layout;

    internal abstract class SButtonBase : Button {
        public SButtonBase() {
            CornerRadius = new(5d);
            Cursor = new(StandardCursorType.Hand);
            FontFamily = MainTheme.Font;
            FontSize = 18d;
            Foreground = MainTheme.NeutralBrush1;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            Margin = new(10d);
            Padding = new(10d);
            VerticalContentAlignment = VerticalAlignment.Center;

            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs args) {
            base.OnPointerPressed(args);
            UpdateBackground();
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs args) {
            base.OnPointerReleased(args);
            UpdateBackground();
        }

        protected abstract void UpdateBackground();

        private void OnPointerEntered(object? sender, PointerEventArgs args) => UpdateBackground();

        private void OnPointerExited(object? sender, PointerEventArgs args) => UpdateBackground();
    }
}
