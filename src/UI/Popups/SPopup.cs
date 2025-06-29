namespace Stonebot.UI.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using Avalonia.Media;

    internal class SPopup : Panel {
        public SPopup(string title, InlineCollection? body, Control footer) {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            IsVisible = false;
            Children.Add(new Border() {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MainTheme.PrimaryBrush2,
                CornerRadius = new(10d),
                Child = new SGrid([
                    GridLength.Auto,
                    GridLength.Auto,
                    GridLength.Auto,
                ], [
                    GridLength.Star,
                ], [
                    new Border{
                        Background = MainTheme.PrimaryBrush1,
                        Child = new STextBlock{
                            Text = title,
                            FontSize = 24d,
                        },
                        CornerRadius = new(10d, 0d),
                    },
                    new STextBlock {
                        TextWrapping = TextWrapping.Wrap,
                        Inlines = body,
                    },
                    footer,
                ]),
                MaxWidth = 700d,
            });
        }
    }
}
