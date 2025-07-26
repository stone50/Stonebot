namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Buttons;
    using Buttons.UserButtons;
    using Popups;

    internal class MainPanel : Panel {
        public readonly MainWindow MainWindow;
        public readonly ConnectButton ConnectButton;
        public readonly BroadcasterButton BroadcasterButton;
        public readonly ChatterButton ChatterButton;
        public readonly AuthorizePopup AuthorizePopup;
        public readonly RemoveAuthorizationPopup RemoveAuthorizationPopup;

        public MainPanel(MainWindow mainWindow) {
            MainWindow = mainWindow;
            Background = MainTheme.PrimaryBrush2;

            RemoveAuthorizationPopup = RemoveAuthorizationPopup.Create();
            AuthorizePopup = AuthorizePopup.Create();
            var logo = GetLogo();
            ConnectButton = new(this);
            BroadcasterButton = new(this);
            ChatterButton = new(this);
            var users = GetUsers(BroadcasterButton, ChatterButton);
            var configButton = GetConfigButton();
            var header = GetHeader(logo, ConnectButton, users, configButton);
            var body = new InteractionGrid();
            var mainGrid = GetMainGrid(header, body);
            Children.Add(mainGrid);
            Children.Add(RemoveAuthorizationPopup);
            Children.Add(AuthorizePopup);
        }

        public void UpdateUsers() {
            BroadcasterButton.UpdateState();
            ChatterButton.UpdateState();
        }

        private static SGrid GetMainGrid(SGrid header, InteractionGrid body) => new([
                GridLength.Auto,
                GridLength.Star,
            ], [
                GridLength.Star,
            ], [
                header,
                body,
            ]);

        private static SGrid GetHeader(Image logo, ConnectButton connectButton, SGrid users, InfoButton configButton) => new([
                GridLength.Star,
            ], [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Star,
                GridLength.Auto,
            ], [
                logo,
                connectButton,
                users,
                configButton,
            ]) {
            Background = MainTheme.PrimaryBrush1,
            Height = 150d,
        };

        private static Image GetLogo() {
            var logo = UIUtils.GetLogo();
            logo.Margin = new(10d);
            return logo;
        }

        private static SGrid GetUsers(BroadcasterButton broadcasterButton, ChatterButton chatterButton) => new([
            GridLength.Star,
            GridLength.Star,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            GetUserLabel("Broadcaster:"),
            broadcasterButton,
            GetUserLabel("Chatter:"),
            chatterButton,
        ]) {
            VerticalAlignment = VerticalAlignment.Center,
        };

        private InfoButton GetConfigButton() {
            var configButton = new InfoButton() {
                Content = UIUtils.GetConfigIcon(),
                VerticalAlignment = VerticalAlignment.Center,
                Height = 75d,
            };
            configButton.Click += (_, _) => {
                IsVisible = false;
                MainWindow.ConfigPanel.Show();
            };
            return configButton;
        }

        private static STextBlock GetUserLabel(string text) => new() {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
    }
}
