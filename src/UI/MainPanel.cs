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
            var newCommandPopup = new NewCommandPopup();
            var addCommandButton = GetAddCommandButton(newCommandPopup);
            var addPatternButton = GetAddPatternButton(/*TODO: pass in popup*/);
            var addTimerButton = GetAddTimerButton(/*TODO: pass in popup*/);
            var subheader = GetSubheader(addCommandButton, addPatternButton, addTimerButton);
            var subheaderBorder = GetSubheaderBorder(subheader);
            var deleteCommandPopup = new DeleteCommandPopup();
            loadableInteractionGrid = GetLoadableInteractionGrid(interactionGrid, deleteCommandPopup);
            var body = GetBody(loadableInteractionGrid);
            var mainGrid = GetMainGrid(header, subheaderBorder, body);
            Children.Add(mainGrid);
            Children.Add(clearAuthPopup);
            Children.Add(confirmAuthPopup);
            Children.Add(newCommandPopup);
            Children.Add(deleteCommandPopup);
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

        private readonly SSelectableTextBlock authorizedLabel;
        private readonly Loadable loadableAuthButtons;
        private readonly Swappable swappableAuthButtons;
        private bool isShowingAuth = false;
        private readonly Loadable loadableInteractionGrid;

        private static SGrid GetMainGrid(SGrid header, Border subheader, ScrollViewer body) => new([
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Star,
        ], [
            GridLength.Star,
        ], [
            header,
            subheader,
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

        private static SSelectableTextBlock GetAuthorizedLabel() => new() {
            FontSize = 24d,
        };

        private static DangerButton GetClearAuthButton(ActionPopup clearAuthPopup) {
            var clearAuthButton = new DangerButton() {
                Content = UIUtils.GetTrashIcon(),
                Height = 32d,
                Padding = new(7d),
            };
            clearAuthButton.Click += (_, _) => clearAuthPopup.IsVisible = true;
            return clearAuthButton;
        }

        private ActionPopup GetClearAuthPopup() => new("Clear Cached Authorization?", [
            new Run("This will only remove cached authorization data. To disconnect your bot from Twitch, go to "),
            new UrlLink("https://www.twitch.tv/settings/connections").GetInline(),
            new Run("."),
        ], () => {
            try {
                Cache.ClearAuthData();
                UpdateAuth();
            } catch (Exception e) {
                Logger.Error(e);
            }
        });

        private static SGrid GetAuthorizedGrid(SSelectableTextBlock authorizedLabel, DangerButton clearAuthButton) => new([
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

        private static SGrid GetSubheader(SuccessButton addCommandButton, SuccessButton addPatternButton, SuccessButton addTimerButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Auto,
            //GridLength.Auto,
            //GridLength.Auto,
        ], [
            addCommandButton,
            //addPatternButton,
            //addTimerButton,
        ]);

        private static Border GetSubheaderBorder(SGrid subheader) => new() {
            Child = subheader,
            BorderBrush = MainTheme.PrimaryBrush4,
            BorderThickness = new(0d, 2d),
        };

        private static SuccessButton GetAddCommandButton(NewCommandPopup newCommandPopup) {
            var addCommandButton = new SuccessButton() {
                Content = "New Command",
            };
            addCommandButton.Click += (_, _) => newCommandPopup.Show();
            return addCommandButton;
        }

        private static SuccessButton GetAddPatternButton(/*TODO: pass in popup*/) {
            var addPatternButton = new SuccessButton() {
                Content = "New Pattern",
            };
            addPatternButton.Click += (_, _) => {/*TODO: show popup*/};
            return addPatternButton;
        }

        private static SuccessButton GetAddTimerButton(/*TODO: pass in popup*/) {
            var addTimerButton = new SuccessButton() {
                Content = "New Timer",
            };
            addTimerButton.Click += (_, _) => {/*TODO: show popup*/};
            return addTimerButton;
        }

        private static Loadable GetLoadableInteractionGrid(InteractionGrid interactionGrid, DeleteCommandPopup deleteCommandPopup) => new(interactionGrid, () => interactionGrid.Init(deleteCommandPopup));

        private static ScrollViewer GetBody(Loadable loadableInteractionGrid) => new() {
            Content = loadableInteractionGrid,
        };
    }
}
