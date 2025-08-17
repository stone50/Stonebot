namespace Stonebot.UI.CustomControls.Buttons {
    using Avalonia.Media;

    internal class SButton : SButtonBase {
        public readonly IImmutableBrush DefaultBrush;
        public readonly IImmutableBrush HoveredBrush;
        public readonly IImmutableBrush PressedBrush;

        public SButton(
            IImmutableBrush defaultBrush,
            IImmutableBrush hoveredBrush,
            IImmutableBrush pressedBrush
        ) {
            Background = defaultBrush;

            DefaultBrush = defaultBrush;
            HoveredBrush = hoveredBrush;
            PressedBrush = pressedBrush;
        }

        protected override void UpdateBackground() => Background =
            IsPressed
                ? PressedBrush
            : IsPointerOver
                ? HoveredBrush
                : DefaultBrush;
    }
}
