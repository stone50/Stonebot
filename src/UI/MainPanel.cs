namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Command.CommandCardControls;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Buttons.Links;
    using CustomControls.Popups;
    using Models.Responses;
    using System.Diagnostics;
    using Twitch;

    internal class MainPanel : Panel {
        public MainPanel(Swappable swappableContent) {
            Background = MainTheme.PrimaryBrush1;

            var logo = GetLogo();
            var connectButton = new ConnectButton();
            swappableAuthButtons = GetSwappableAuthButtons();
            var confirmAuthPopup = new ConfirmAuthPopup();
            var authButton = GetAuthButton(confirmAuthPopup);
            authorizedLabel = GetAuthorizedLabel();
            var clearAuthPopup = GetClearAuthPopup();
            var clearAuthButton = GetClearAuthButton(clearAuthPopup);
            var authorizedGrid = GetAuthorizedGrid(authorizedLabel, clearAuthButton);
            swappableAuthButtons.Init(authButton, authorizedGrid);
            loadableAuthButtons = GetLoadableAuthButtons(swappableAuthButtons);
            var configButton = GetConfigButton(swappableContent);
            var header = GetHeader(logo, connectButton, loadableAuthButtons, configButton);
            var interactionGrid = new InteractionGrid();
            var newCommandPopup = new NewCommandPopup(interactionGrid);
            loadableInteractionGrid = GetLoadableInteractionGrid(interactionGrid, newCommandPopup);
            var body = GetBody(loadableInteractionGrid);
            var mainGrid = GetMainGrid(header, body);
            Children.Add(mainGrid);
            Children.Add(clearAuthPopup);
            Children.Add(confirmAuthPopup);
            Children.Add(newCommandPopup);
        }

        public void LoadAuth() => loadableAuthButtons.Load();

        public void UpdateAuth() {
            if (Cache.IsAuthorized) {
                if (!isShowingAuth) {
                    swappableAuthButtons.Swap();
                    isShowingAuth = true;
                }

                UpdateAuthorizedLabelText();
                return;
            }

            if (isShowingAuth) {
                swappableAuthButtons.Swap();
                isShowingAuth = false;
            }
        }

        public void LoadInteractionGrid() => loadableInteractionGrid.Load();

        private readonly STextBlock authorizedLabel;
        private readonly Loadable loadableAuthButtons;
        private readonly Swappable swappableAuthButtons;
        private bool isShowingAuth = false;
        private readonly Loadable loadableInteractionGrid;

        private static SGrid GetMainGrid(SGrid header, ScrollViewer body) => new([
            GridLength.Auto,
            GridLength.Star,
        ], [
            GridLength.Star,
        ], [
            header,
            body,
        ]);

        private static SGrid GetHeader(Image logo, ConnectButton connectButton, Loadable loadableAuthButtons, InfoButton configButton) => new([
            GridLength.Star,
        ], [
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Star,
            GridLength.Auto,
        ], [
            logo,
            connectButton,
            loadableAuthButtons,
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

        private static Swappable GetSwappableAuthButtons() => new() {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };

        private InfoButton GetAuthButton(ConfirmAuthPopup confirmAuthPopup) {
            var authButton = new InfoButton() {
                Content = "Authorize",
            };
            authButton.Click += GetOnAuthButtonClick(confirmAuthPopup);
            return authButton;
        }

        private EventHandler<RoutedEventArgs> GetOnAuthButtonClick(ConfirmAuthPopup confirmAuthPopup) => (_, _) => {
            PostDeviceCodeResponse deviceCodeResponse;
            try {
                deviceCodeResponse = Auth.GetDeviceCode();
            } catch (Exception e) {
                Logger.Error(e);
                return;
            }

            try {
                _ = Process.Start(new ProcessStartInfo {
                    FileName = deviceCodeResponse.VerificationUri,
                    UseShellExecute = true,
                });
            } catch (Exception e) {
                Logger.Error(e);
            }

            confirmAuthPopup.Show(deviceCodeResponse.UserCode, deviceCodeResponse.VerificationUri,
                () => {
                    try {
                        Cache.LoadNewAccessToken(deviceCodeResponse.DeviceCode);
                        UpdateAuth();
                    } catch (Exception e) {
                        Logger.Error(e);
                    }
                }
            );
        };

        private static STextBlock GetAuthorizedLabel() => new() {
            FontSize = 24d,
        };

        private static DangerButton GetClearAuthButton(ActionPopup clearAuthPopup) {
            var clearAuthButton = new DangerButton() {
                Content = "Clear", // TODO: make this a trash can icon
            };
            clearAuthButton.Click += (_, _) => clearAuthPopup.IsVisible = true;
            return clearAuthButton;
        }

        private ActionPopup GetClearAuthPopup() => new("Clear Cached Authorization?", [
            new Run("This will only remove cached authorization data.\nTo disconnect Stonebot from Twitch, go to:\n"),
            new UrlLink("https://www.twitch.tv/settings/connections").GetInline(),
        ], () => {
            try {
                Cache.ClearAuthData();
                UpdateAuth();
            } catch (Exception e) {
                Logger.Error(e);
            }
        });

        private static SGrid GetAuthorizedGrid(STextBlock authorizedLabel, DangerButton clearAuthButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            GridLength.Auto,
        ], [
            authorizedLabel,
            clearAuthButton,
        ]);

        private Loadable GetLoadableAuthButtons(Swappable swappableAuthButtons) => new(swappableAuthButtons, UpdateAuth);

        private void UpdateAuthorizedLabelText() => authorizedLabel.Text = $"Chatting as {Cache.GetChatterDisplayName()}";

        private static InfoButton GetConfigButton(Swappable swappableContent) {
            var configButton = new InfoButton() {
                Content = UIUtils.GetConfigIcon(),
                Height = 75d,
                VerticalAlignment = VerticalAlignment.Center,
            };
            configButton.Click += (_, _) => swappableContent.Swap();
            return configButton;
        }

        private static Loadable GetLoadableInteractionGrid(InteractionGrid interactionGrid, NewCommandPopup newCommandPopup) => new(interactionGrid, () => interactionGrid.Init(newCommandPopup));

        private static ScrollViewer GetBody(Loadable loadableInteractionGrid) => new() {
            Content = loadableInteractionGrid,
        };
    }
}
