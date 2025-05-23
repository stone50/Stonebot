namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Input;

    internal abstract class SButtonBase : Button {
        public SButtonBase() => Cursor = new(StandardCursorType.Hand);

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
