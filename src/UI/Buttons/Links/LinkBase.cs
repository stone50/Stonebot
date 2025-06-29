namespace Stonebot.UI.Buttons.Links {
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using Avalonia.Media;
    using System.Diagnostics;

    internal abstract class LinkBase : InfoButton {
        public readonly string ContentText;

        public LinkBase(string contentText) {
            ContentText = contentText;
            Content = new STextBlock() {
                Text = ContentText,
                TextDecorations = TextDecorations.Underline,
                TextWrapping = TextWrapping.Wrap,
                Foreground = MainTheme.InfoBrush2,
                Padding = new(0d),
                Margin = new(0d),
            };
            Background = null;
            VerticalAlignment = VerticalAlignment.Bottom;
            Padding = new(0d);
            Margin = new(0d);
        }

        public InlineUIContainer GetInline() => new(this);

        protected abstract ProcessStartInfo GetProcessStartInfo();

        protected override void OnClick() {
            Utils.TryElseError(() => Process.Start(GetProcessStartInfo()));
            base.OnClick();
        }

        protected override void UpdateBackground() => ((STextBlock)Content!).Foreground =
            IsPressed
                ? PressedBrush
            : IsPointerOver
                ? HoveredBrush
                : DefaultBrush;
    }
}
