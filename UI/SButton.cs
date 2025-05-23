namespace Stonebot.UI {
    using Avalonia.Media;

    internal class SButton : SButtonBase {
        public readonly IImmutableBrush DefaultBrush;
        public readonly IImmutableBrush HoveredBrush;
        public readonly IImmutableBrush PressedBrush;

        public SButton(IImmutableBrush defaultBrush, IImmutableBrush hoveredBrush, IImmutableBrush pressedBrush) {
            DefaultBrush = defaultBrush;
            HoveredBrush = hoveredBrush;
            PressedBrush = pressedBrush;
            Background = defaultBrush;
        }

        protected override void UpdateBackground() => Background =
            IsPressed
                ? PressedBrush
            : IsPointerOver
                ? HoveredBrush
                : DefaultBrush;
    }
}
