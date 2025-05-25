namespace Stonebot.UI.Buttons {
    using Avalonia.Media;

    internal class SButton : SButtonBase {
        public readonly IImmutableBrush DefaultBrush;
        public readonly IImmutableBrush HoveredBrush;
        public readonly IImmutableBrush PressedBrush;

        public SButton(
            MainWindow mainWindow,
            IImmutableBrush defaultBrush,
            IImmutableBrush hoveredBrush,
            IImmutableBrush pressedBrush
        ) : base(mainWindow) {
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
