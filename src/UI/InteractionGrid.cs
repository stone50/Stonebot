namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;

    internal class InteractionGrid : WrapPanel {
        public InteractionGrid() {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new(10d);
        }
    }
}
