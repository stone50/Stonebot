namespace Stonebot.UI.CustomControls {
    using Avalonia.Controls;
    using Avalonia.Layout;

    internal class STextBlock : TextBlock {
        public STextBlock() {
            FontFamily = MainTheme.Font;
            FontSize = 18d;
            Foreground = MainTheme.NeutralBrush1;
            HorizontalAlignment = HorizontalAlignment.Center;
            Margin = new(10d);
            VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
