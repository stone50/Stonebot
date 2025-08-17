namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Buttons.UserButtons;
    using CustomControls.Popups;

    internal class MainPanel : Panel {
        public MainPanel(Swappable swappableContent) {
            Background = MainTheme.PrimaryBrush1;

            var removeAuthorizationPopup = RemoveAuthorizationPopup.Create();
            var cancelAuthorizationPopup = CancelPopup.Create("Please Authorize in your Browser");
            var logo = GetLogo();
            var connectButton = new ConnectButton();
            broadcasterButton = new(removeAuthorizationPopup, cancelAuthorizationPopup);
            chatterButton = new(removeAuthorizationPopup, cancelAuthorizationPopup);
            var broadcasterLabel = GetUserLabel("Broadcaster:");
            var chatterLabel = GetUserLabel("Chatter:");
            var users = GetUsers(broadcasterLabel, chatterLabel);
            var configButton = GetConfigButton(swappableContent);
            var header = GetHeader(logo, connectButton, users, configButton);
            interactionGrid = new InteractionGrid();
            var body = GetBody();
            var mainGrid = GetMainGrid(header, body);
            Children.Add(mainGrid);
            Children.Add(removeAuthorizationPopup);
            Children.Add(cancelAuthorizationPopup);
        }

        public void UpdateBroadcasterButton() => broadcasterButton.Update();

        public void UpdateChatterButton() => chatterButton.Update();

        public void InitInteractionGrid() => interactionGrid.Init();

        private readonly BroadcasterButton broadcasterButton;
        private readonly ChatterButton chatterButton;
        private readonly InteractionGrid interactionGrid;

        private static SGrid GetMainGrid(SGrid header, ScrollViewer body) => new([
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
            Background = MainTheme.PrimaryBrush2,
            Height = 150d,
        };

        private static Image GetLogo() {
            var logo = UIUtils.GetLogo();
            logo.Margin = new(10d);
            return logo;
        }

        private SGrid GetUsers(STextBlock broadcasterLabel, STextBlock chatterLabel) => new([
            GridLength.Star,
            GridLength.Star,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            broadcasterLabel,
            broadcasterButton,
            chatterLabel,
            chatterButton,
        ]) {
            VerticalAlignment = VerticalAlignment.Center,
        };

        private static InfoButton GetConfigButton(Swappable swappableContent) {
            var configButton = new InfoButton() {
                Content = UIUtils.GetConfigIcon(),
                Height = 75d,
                VerticalAlignment = VerticalAlignment.Center,
            };
            configButton.Click += (_, _) => swappableContent.Swap();
            return configButton;
        }

        private static STextBlock GetUserLabel(string text) => new() {
            HorizontalAlignment = HorizontalAlignment.Right,
            Text = text,
        };

        private ScrollViewer GetBody() => new() {
            Content = interactionGrid,
        };
    }
}
