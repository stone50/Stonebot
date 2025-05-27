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
            ConfigPanel = new(this) {
                IsVisible = false,
            };
            Content = new Panel {
                Children = {
                    MainPanel,
                    ConfigPanel,
                }
            };
        }
    }
}
