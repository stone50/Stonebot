namespace Stonebot.UI.Buttons {
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using System.Diagnostics;

    internal class Link : SButton {
        public Link() : base(MainTheme.InfoBrush2, MainTheme.InfoBrush1, MainTheme.InfoBrush3) {
            Foreground = MainTheme.InfoBrush2;
            Background = null;
            BorderThickness = new(0d);
            VerticalAlignment = VerticalAlignment.Bottom;
            Padding = new(0d);
        }

        public InlineUIContainer GetInline() => new(this);

        protected override void OnClick() {
            _ = Process.Start(new ProcessStartInfo {
                FileName = Content?.ToString(),
                UseShellExecute = true
            });

            base.OnClick();
        }

        protected override void UpdateBackground() => Foreground =
            IsPressed
                ? PressedBrush
            : IsPointerOver
                ? HoveredBrush
                : DefaultBrush;
    }
}
