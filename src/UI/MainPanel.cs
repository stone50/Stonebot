namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Buttons.Links;
    using CustomControls.Popups;

    internal class MainPanel : Panel {
        public MainPanel(Swappable swappableContent) {
            Background = MainTheme.PrimaryBrush1;

            var clearAuthPopup = GetClearAuthPopup();
            var confirmAuthPopup = GetConfirmAuthPopup();
            var logo = GetLogo();
            var connectButton = new ConnectButton();
            authorizeButton = new AuthorizeButton(clearAuthPopup, confirmAuthPopup);
            var configButton = GetConfigButton(swappableContent);
            var header = GetHeader(logo, connectButton, authorizeButton, configButton);
            interactionGrid = new InteractionGrid();
            var body = GetBody();
            var mainGrid = GetMainGrid(header, body);
            Children.Add(mainGrid);
            Children.Add(clearAuthPopup);
            Children.Add(confirmAuthPopup);
        }

        public void UpdateAuthorizeButton() => authorizeButton.Update();

        public void InitInteractionGrid() => interactionGrid.Init();

        private readonly AuthorizeButton authorizeButton;
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

        private static SGrid GetHeader(Image logo, ConnectButton connectButton, AuthorizeButton authorizeButton, InfoButton configButton) => new([
                GridLength.Star,
            ], [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Star,
                GridLength.Auto,
            ], [
                logo,
                connectButton,
                authorizeButton,
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

        private static CancelOkPopup GetClearAuthPopup() => CancelOkPopup.Create("Clear Cached Authorization?", [
            new Run("This will only remove cached authorization data.\nTo disconnect Stonebot from Twitch, go to:\n"),
            new UrlLink("https://www.twitch.tv/settings/connections").GetInline(),
            new Run("\nMake sure you are logged into the correct user."),
        ]);

        private static ConfirmAuthPopup GetConfirmAuthPopup() => ConfirmAuthPopup.Create("Authorize");

        private static InfoButton GetConfigButton(Swappable swappableContent) {
            var configButton = new InfoButton() {
                Content = UIUtils.GetConfigIcon(),
                Height = 75d,
                VerticalAlignment = VerticalAlignment.Center,
            };
            configButton.Click += (_, _) => swappableContent.Swap();
            return configButton;
        }

        private ScrollViewer GetBody() => new() {
            Content = interactionGrid,
        };
    }
}
