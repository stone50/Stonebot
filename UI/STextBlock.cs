namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;

    internal class STextBlock : TextBlock {
        public STextBlock() {
            FontFamily = MainTheme.Font;
            Foreground = MainTheme.NeutralBrush1;
            FontSize = 18d;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Margin = new(10d);
        }
    }
}
