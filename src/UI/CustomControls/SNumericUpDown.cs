namespace Stonebot.UI.CustomControls {
    using Avalonia.Controls;
    using Avalonia.Layout;

    internal class SNumericUpDown : NumericUpDown {
        public SNumericUpDown() {
            FontFamily = MainTheme.Font;
            FontSize = 18d;
            Foreground = MainTheme.NeutralBrush1;
            HorizontalAlignment = HorizontalAlignment.Left;
            Increment = 1M;
            Margin = new(10d);
            ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
            VerticalAlignment = VerticalAlignment.Center;
        }

        public SNumericUpDown(decimal min, decimal max, bool showSpinner) : this() {
            AllowSpin = showSpinner;
            Maximum = max;
            Minimum = min;
            ShowButtonSpinner = showSpinner;
        }

        protected override Type StyleKeyOverride => typeof(NumericUpDown);
    }
}
