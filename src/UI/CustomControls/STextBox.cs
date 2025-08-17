namespace Stonebot.UI.CustomControls {
    using Avalonia.Controls;
    using Avalonia.Layout;

    internal class STextBox : TextBox {
        public STextBox() {
            FontFamily = MainTheme.Font;
            FontSize = 18d;
            Foreground = MainTheme.NeutralBrush1;
            HorizontalAlignment = HorizontalAlignment.Center;
            Margin = new(10d);
            VerticalAlignment = VerticalAlignment.Center;
        }

        protected override Type StyleKeyOverride => typeof(TextBox);
    }
}
