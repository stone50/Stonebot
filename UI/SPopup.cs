namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Media;

    internal class SPopup : Panel {
        public readonly MainPanel MainPanel;

        public SPopup(MainPanel mainPanel) {
            MainPanel = mainPanel;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            IsVisible = false;
        }
    }
}
