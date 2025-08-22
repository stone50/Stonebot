namespace Stonebot.UI.CustomControls.Buttons.Links {
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Avalonia.Media;
    using Helpers;
    using System.Diagnostics;

    internal abstract class LinkBase : InfoButton {
        public STextBlock TextBlock => (STextBlock)Content!;

        public LinkBase() {
            Background = null;
            Content = GetContent();
            Margin = new(0d);
            Padding = new(0d);
            VerticalAlignment = VerticalAlignment.Bottom;

            Click += OnClick;
        }

        public LinkBase(string text) : this() => TextBlock.Text = text;

        public InlineUIContainer GetInline() => new(this);

        protected abstract ProcessStartInfo GetProcessStartInfo();

        protected override void UpdateBackground() => ((STextBlock)Content!).Foreground =
            IsPressed
                ? PressedBrush
            : IsPointerOver
                ? HoveredBrush
                : DefaultBrush;

        private void OnClick(object? sender, RoutedEventArgs e) => ExceptionHelper.TryElseError(() => Process.Start(GetProcessStartInfo()));

        private static STextBlock GetContent() => new() {
            Foreground = MainTheme.InfoBrush2,
            Margin = new(0d),
            Padding = new(0d),
            TextDecorations = TextDecorations.Underline,
            TextWrapping = TextWrapping.Wrap,
        };
    }
}
