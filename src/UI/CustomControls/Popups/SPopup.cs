namespace Stonebot.UI.CustomControls.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using Avalonia.Media;

    internal class SPopup : Panel {
        public SPopup(string title, InlineCollection? bodyInlines, Control footer) {
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            HorizontalAlignment = HorizontalAlignment.Stretch;
            IsVisible = false;
            VerticalAlignment = VerticalAlignment.Stretch;

            var headerTextBlock = GetHeaderTextBlock(title);
            var header = GetHeader(headerTextBlock);
            var body = GetBody(bodyInlines);
            var mainBorderChild = GetMainBorderChild(header, body, footer);
            var mainBorder = GetMainBorder(mainBorderChild);
            Children.Add(mainBorder);
        }

        private static Border GetMainBorder(SGrid mainBorderChild) => new() {
            Background = MainTheme.PrimaryBrush2,
            Child = mainBorderChild,
            CornerRadius = new(10d),
            HorizontalAlignment = HorizontalAlignment.Center,
            MaxWidth = 700d,
            VerticalAlignment = VerticalAlignment.Center,
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

        private static Border GetHeader(STextBlock headerTextBlock) => new() {
            Background = MainTheme.PrimaryBrush1,
            Child = headerTextBlock,
            CornerRadius = new(10d, 0d),
        };

        private static STextBlock GetHeaderTextBlock(string title) => new() {
            FontSize = 24d,
            Text = title,
        };

        private static STextBlock GetBody(InlineCollection? inlines) => new() {
            Inlines = inlines,
            TextWrapping = TextWrapping.Wrap,
        };
    }
}
