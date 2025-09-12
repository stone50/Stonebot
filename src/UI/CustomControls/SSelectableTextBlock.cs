namespace Stonebot.UI.CustomControls {
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Layout;

    internal class SSelectableTextBlock : SelectableTextBlock {
        public SSelectableTextBlock() {
            Cursor = new(StandardCursorType.Ibeam);
            FontFamily = MainTheme.Font;
            FontSize = 18d;
            Foreground = MainTheme.NeutralBrush1;
            HorizontalAlignment = HorizontalAlignment.Center;
            Margin = new(10d);
            SelectionBrush = MainTheme.InfoBrush2;
            VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
