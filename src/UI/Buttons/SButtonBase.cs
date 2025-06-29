namespace Stonebot.UI.Buttons {
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Layout;

    internal abstract class SButtonBase : Button {
        public SButtonBase() {
            FontFamily = MainTheme.Font;
            Foreground = MainTheme.NeutralBrush1;
            FontSize = 18d;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
            CornerRadius = new(5d);
            Padding = new(10d);
            Margin = new(10d);
            Cursor = new(StandardCursorType.Hand);
        }

        protected override void OnPointerEntered(PointerEventArgs args) => UpdateBackground();

        protected override void OnPointerExited(PointerEventArgs args) => UpdateBackground();

        protected override void OnPointerPressed(PointerPressedEventArgs args) {
            base.OnPointerPressed(args);
            UpdateBackground();
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs args) {
            base.OnPointerReleased(args);
            UpdateBackground();
        }

        protected abstract void UpdateBackground();
    }
}
