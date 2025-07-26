namespace Stonebot.UI.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using Avalonia.Media;

    internal class SPopup : Panel {
        public SPopup(string title, InlineCollection? bodyInlines, Control footer) {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            IsVisible = false;
            var header = GetHeader(title);
            var body = GetBody(bodyInlines);
            var mainBorderChild = GetMainBorderChild(header, body, footer);
            var mainBorder = GetMainBorder(mainBorderChild);
            Children.Add(mainBorder);
        }

        private static Border GetMainBorder(SGrid child) => new() {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Background = MainTheme.PrimaryBrush2,
            CornerRadius = new(10d),
            Child = child,
            MaxWidth = 700d,
        };

        private static SGrid GetMainBorderChild(Border header, STextBlock body, Control footer) => new([
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GridLength.Star,
            ], [
                header,
                body,
                footer,
            ]);

        private static Border GetHeader(string title) => new() {
            Background = MainTheme.PrimaryBrush1,
            Child = new STextBlock {
                Text = title,
                FontSize = 24d,
            },
            CornerRadius = new(10d, 0d),
        };

        private static STextBlock GetBody(InlineCollection? inlines) => new() {
            TextWrapping = TextWrapping.Wrap,
            Inlines = inlines,
        };
    }
}
