namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;

    internal class SNumericUpDown : NumericUpDown {
        public SNumericUpDown() {
            Increment = 1M;
            ParsingNumberStyle = System.Globalization.NumberStyles.Integer;
            FontFamily = MainTheme.Font;
            FontSize = 18d;
            Foreground = MainTheme.NeutralBrush1;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Center;
            Margin = new(10d);
        }

        public SNumericUpDown(decimal min, decimal max, bool showSpinner) : this() {
            Minimum = min;
            Maximum = max;
            AllowSpin = showSpinner;
            ShowButtonSpinner = showSpinner;
        }

        protected override Type StyleKeyOverride => typeof(NumericUpDown);
    }
}
