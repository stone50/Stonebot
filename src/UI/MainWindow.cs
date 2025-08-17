namespace Stonebot.UI {
    using Avalonia.Controls;
    using CustomControls;

    internal class MainWindow : Window {
        public MainWindow() {
            Focusable = true;
            Height = 800d;
            Title = "Stonebot";
            Width = 1000d;
            WindowState = WindowState.Maximized;

            var swappableContent = new Swappable();
            mainPanel = new(swappableContent);
            configPanel = new(swappableContent, mainPanel);
            swappableContent.Init(mainPanel, configPanel);
            Content = swappableContent;
        }

        public void UpdateMainPanelUserButtons() {
            mainPanel.UpdateBroadcasterButton();
            mainPanel.UpdateChatterButton();
        }

        public void UpdateMainPanelInteractionGrid() => mainPanel.InitInteractionGrid();

        public void InitConfigPanel() => configPanel.Init();

        private readonly MainPanel mainPanel;
        private readonly ConfigPanel configPanel;
    }
}
