namespace Stonebot.UI {
    using Avalonia.Controls;

    internal class MainWindow : Window {
        public readonly MainPanel MainPanel;
        public readonly ConfigPanel ConfigPanel;

        public MainWindow() {
            Title = "Stonebot";
            Width = 1000d;
            Height = 800d;
            MainPanel = new(this);
            ConfigPanel = GetConfigPanel();
            Content = GetContent(MainPanel, ConfigPanel);
            WindowState = WindowState.Maximized;
        }

        private ConfigPanel GetConfigPanel() => new(this) {
            IsVisible = false
        };

        private static Panel GetContent(MainPanel mainPanel, ConfigPanel configPanel) => new() {
            Children = {
                mainPanel,
                configPanel,
            }
        };
    }
}
