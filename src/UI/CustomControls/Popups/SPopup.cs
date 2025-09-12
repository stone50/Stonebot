namespace Stonebot.UI.CustomControls.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using Avalonia.Media;

    internal class SPopup : Panel {
        public readonly Border Header;
        public readonly Panel Body;
        public readonly Panel Footer;

        public SPopup() {
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            HorizontalAlignment = HorizontalAlignment.Stretch;
            IsVisible = false;
            VerticalAlignment = VerticalAlignment.Stretch;

            Header = GetHeader();
            Body = GetBody();
            Footer = GetFooter();
            var mainBorderChild = GetMainBorderChild(Header, Body, Footer);
            var mainBorder = GetMainBorder(mainBorderChild);
            Children.Add(mainBorder);
        }

        public SPopup(string title) : this() => Header.Child = new SSelectableTextBlock() {
            FontSize = 24d,
            Text = title,
        };

        public SPopup(string title, InlineCollection inlines) : this(title) => Body.Children.Add(new SSelectableTextBlock() {
            Inlines = inlines,
            TextWrapping = TextWrapping.Wrap,
        });

        private static Border GetMainBorder(SGrid mainBorderChild) => new() {
            Background = MainTheme.PrimaryBrush2,
            Child = mainBorderChild,
            CornerRadius = new(10d),
            HorizontalAlignment = HorizontalAlignment.Center,
            MaxWidth = 700d,
            VerticalAlignment = VerticalAlignment.Center,
        };

        private static SGrid GetMainBorderChild(Border header, Panel body, Panel footer) => new([
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

        private static Border GetHeader() => new() {
            Background = MainTheme.PrimaryBrush1,
            CornerRadius = new(10d, 0d),
        };

        private static Panel GetBody() => new();

        private static Panel GetFooter() => new();
    }
}
