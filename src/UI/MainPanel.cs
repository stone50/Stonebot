namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Buttons;
    using Stonebot.UI.Buttons.UserButtons;
    using Stonebot.UI.Popups;

    internal class MainPanel : Panel {
        public readonly MainWindow MainWindow;
        public readonly ConnectButton ConnectButton;
        public readonly BroadcasterButton BroadcasterButton;
        public readonly ChatterButton ChatterButton;
        public readonly AuthorizePopup AuthorizePopup;
        public readonly RemoveAuthorizationPopup RemoveAuthorizationPopup;

        public MainPanel(MainWindow mainWindow) {
            MainWindow = mainWindow;
            ConnectButton = new(this);
            RemoveAuthorizationPopup = RemoveAuthorizationPopup.Create();
            AuthorizePopup = AuthorizePopup.Create();
            BroadcasterButton = new(this);
            ChatterButton = new(this);
            var users = new SGrid([
                GridLength.Star,
                GridLength.Star,
            ], [
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GetUserLabel("Broadcaster:"),
                BroadcasterButton,
                GetUserLabel("Chatter:"),
                ChatterButton,
            ]) {
                VerticalAlignment = VerticalAlignment.Center,
            };
            var configButton = new InfoButton() {
                Content = UIUtils.GetConfigIcon(),
                VerticalAlignment = VerticalAlignment.Center,
                Height = 75d,
            };
            configButton.Click += (_, _) => {
                IsVisible = false;
                MainWindow.ConfigPanel.Show();
            };
            var logo = UIUtils.GetLogo();
            logo.Margin = new(10d);
            var header = new SGrid([
                GridLength.Star,
            ], [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Star,
                GridLength.Auto,
            ], [
                logo,
                ConnectButton,
                users,
                configButton,
            ]) {
                Background = MainTheme.PrimaryBrush1,
                Height = 150d,
            };
            var body = new InteractionGrid();
            var fullGrid = new SGrid([
                GridLength.Auto,
                GridLength.Star,
            ], [
                GridLength.Star,
            ], [
                header,
                body,
            ]);
            Background = MainTheme.PrimaryBrush2;
            Children.Add(fullGrid);
            Children.Add(RemoveAuthorizationPopup);
            Children.Add(AuthorizePopup);
        }

        public void UpdateUsers() {
            BroadcasterButton.UpdateState();
            ChatterButton.UpdateState();
        }

        private static STextBlock GetUserLabel(string text) => new() {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
    }
}
