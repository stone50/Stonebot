namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Media;

    internal class SPopup : Panel {
        public readonly MainWindow MainWindow;

        public SPopup(MainWindow mainWindow) {
            MainWindow = mainWindow;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            IsVisible = false;
        }
    }
}
