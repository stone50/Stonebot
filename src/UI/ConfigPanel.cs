namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Buttons;
    using Buttons.Links;
    using Popups;

    internal class ConfigPanel : Panel {
        public readonly MainWindow MainWindow;
        public readonly TextBox BroadcasterClientIdInput;
        public readonly TextBox BroadcasterClientSecretInput;
        public readonly TextBox ChatterClientIdInput;
        public readonly TextBox ChatterClientSecretInput;
        public readonly NumericUpDown AuthorizationPortInput;
        public readonly NumericUpDown NumMaxLogFilesInput;
        public readonly NumericUpDown WebSocketConnectTimeoutSecondsInput;
        public readonly NumericUpDown WebSocketKeepaliveTimeoutSecondsInput;
        public readonly NumericUpDown WebSocketKeepaliveTimeoutMarginSecondsInput;

        public ConfigPanel(MainWindow mainWindow) {
            MainWindow = mainWindow;
            Background = MainTheme.PrimaryBrush2;

            var configIcon = UIUtils.GetConfigIcon();
            var headerTitle = GetHeaderTitle();
            var cancelButton = GetCancelButton();
            var saveButton = GetSaveButton();
            var header = GetHeader(configIcon, headerTitle, cancelButton, saveButton);

            BroadcasterClientIdInput = GetMaskedConfigValueTextBox();
            var broadcasterClientIdPopup = GetBasicConfigValueInfoPopup("Broadcaster Client ID", "client ID", "broadcaster");

            BroadcasterClientSecretInput = GetMaskedConfigValueTextBox();
            var broadcasterClientSecretPopup = GetBasicConfigValueInfoPopup("Broadcaster Client Secret", "client secret", "broadcaster");

            ChatterClientIdInput = GetMaskedConfigValueTextBox();
            var chatterClientIdPopup = GetBasicConfigValueInfoPopup("Chatter Client ID", "client ID", "chatter");

            ChatterClientSecretInput = GetMaskedConfigValueTextBox();
            var chatterClientSecretPopup = GetBasicConfigValueInfoPopup("Chatter Client Secret", "client secret", "chatter");

            var basicConfigGrid = GetBasicConfigGrid([
                GetConfigValueLabel("Broadcaster Client ID"),
                GetConfigValueInfoButton(broadcasterClientIdPopup),
                BroadcasterClientIdInput,

                GetConfigValueLabel("Broadcaster Client Secret"),
                GetConfigValueInfoButton(broadcasterClientSecretPopup),
                BroadcasterClientSecretInput,

                GetConfigValueLabel("Chatter Client ID"),
                GetConfigValueInfoButton(chatterClientIdPopup),
                ChatterClientIdInput,

                GetConfigValueLabel("Chatter Client Secret"),
                GetConfigValueInfoButton(chatterClientSecretPopup),
                ChatterClientSecretInput,
            ]);

            var bodySeparator = GetBodySeparator();

            var authorizationPortPopup = GetAuthorizationPortPopup();
            AuthorizationPortInput = GetNumericUpDown(Constants.AuthorizationPortMin, Constants.AuthorizationPortMax, false);

            var numMaxLogFilesPopup = GetNumMaxLogFilesPopup();
            NumMaxLogFilesInput = GetNumericUpDown(Constants.NumMaxLogFilesMin, Constants.NumMaxLogFilesMax, true);

            var webSocketConnectTimeoutSecondsPopup = GetWebSocketConnectTimeoutSecondsPopup();
            WebSocketConnectTimeoutSecondsInput = GetNumericUpDown(Constants.WebSocketConnectTimeoutSecondsMin, Constants.WebSocketConnectTimeoutSecondsMax, true);

            var webSocketKeepaliveTimeoutSecondsPopup = GetWebSocketKeepaliveTimeoutSecondsPopup();
            WebSocketKeepaliveTimeoutSecondsInput = GetNumericUpDown(Constants.WebSocketKeepaliveTimeoutSecondsMin, Constants.WebSocketKeepaliveTimeoutSecondsMax, true);

            var webSocketKeepaliveTimeoutMarginSecondsPopup = GetWebSocketKeepaliveTimeoutMarginSecondsPopup();
            WebSocketKeepaliveTimeoutMarginSecondsInput = GetNumericUpDown(Constants.WebSocketKeepaliveTimeoutMarginSecondsMin, Constants.WebSocketKeepaliveTimeoutMarginSecondsMax, true);

            var advancedConfigGrid = GetAdvancedConfigGrid([
                GetConfigValueLabel("Authorization Port"),
                GetConfigValueInfoButton(authorizationPortPopup),
                AuthorizationPortInput,

                GetConfigValueLabel("Max Log Files"),
                GetConfigValueInfoButton(numMaxLogFilesPopup),
                NumMaxLogFilesInput,

                GetConfigValueLabel("Connect Timeout Seconds"),
                GetConfigValueInfoButton(webSocketConnectTimeoutSecondsPopup),
                WebSocketConnectTimeoutSecondsInput,

                GetConfigValueLabel("Keepalive Timeout Seconds"),
                GetConfigValueInfoButton(webSocketKeepaliveTimeoutSecondsPopup),
                WebSocketKeepaliveTimeoutSecondsInput,

                GetConfigValueLabel("Keepalive Timeout Margin Seconds"),
                GetConfigValueInfoButton(webSocketKeepaliveTimeoutMarginSecondsPopup),
                WebSocketKeepaliveTimeoutMarginSecondsInput,
            ]);

            var body = GetBody(basicConfigGrid, bodySeparator, advancedConfigGrid);
            var mainGrid = GetMainGrid(header, body);
            Children.Add(mainGrid);
            Children.Add(broadcasterClientIdPopup);
            Children.Add(broadcasterClientSecretPopup);
            Children.Add(chatterClientIdPopup);
            Children.Add(chatterClientSecretPopup);
            Children.Add(authorizationPortPopup);
            Children.Add(numMaxLogFilesPopup);
            Children.Add(webSocketConnectTimeoutSecondsPopup);
            Children.Add(webSocketKeepaliveTimeoutSecondsPopup);
            Children.Add(webSocketKeepaliveTimeoutMarginSecondsPopup);
        }

        public void Show() {
            BroadcasterClientIdInput.Text = Config.BroadcasterClientId;
            BroadcasterClientSecretInput.Text = Config.BroadcasterClientSecret;
            ChatterClientIdInput.Text = Config.ChatterClientId;
            ChatterClientSecretInput.Text = Config.ChatterClientSecret;
            AuthorizationPortInput.Value = Config.AuthorizationPort;
            NumMaxLogFilesInput.Value = Config.NumMaxLogFiles;
            WebSocketConnectTimeoutSecondsInput.Value = Config.WebSocketConnectTimeoutSeconds;
            WebSocketKeepaliveTimeoutSecondsInput.Value = Config.WebSocketKeepaliveTimeoutSeconds;
            WebSocketKeepaliveTimeoutMarginSecondsInput.Value = Config.WebSocketKeepaliveTimeoutMarginSeconds;
            IsVisible = true;
        }

        private static SGrid GetMainGrid(SGrid header, SGrid body) => new([
                GridLength.Auto,
                GridLength.Star,
            ], [
                GridLength.Star,
            ], [
                header,
                body,
            ]);

        private static SGrid GetHeader(Image configIcon, STextBlock headerTitle, DangerButton cancelButton, SuccessButton saveButton) => new([
               GridLength.Star
           ], [
               GridLength.Auto,
                GridLength.Star,
                GridLength.Auto,
                GridLength.Auto,
           ], [
               configIcon,
                headerTitle,
                cancelButton,
                saveButton,
           ]) {
            Background = MainTheme.PrimaryBrush1,
            Height = 150d,
        };

        private static STextBlock GetHeaderTitle() => new() {
            Text = "Config",
            FontSize = 48d,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        private DangerButton GetCancelButton() {
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                Height = 60d,
            };
            cancelButton.Click += (_, _) => {
                IsVisible = false;
                MainWindow.MainPanel.IsVisible = true;
            };
            return cancelButton;
        }

        private SuccessButton GetSaveButton() {
            var saveButton = new SuccessButton() {
                Content = "Save",
                Height = 60d
            };
            saveButton.Click += GetOnSaveButtonClick();
            return saveButton;
        }

        private EventHandler<RoutedEventArgs> GetOnSaveButtonClick() => (_, _) => {
            if (BroadcasterClientIdInput.Text != Config.BroadcasterClientId) {
                Config.BroadcasterClientId = BroadcasterClientIdInput.Text!;
                Cache.ClearBroadcasterAuthorizationData();
                MainWindow.MainPanel.BroadcasterButton.UpdateState();
            }

            if (BroadcasterClientSecretInput.Text != Config.BroadcasterClientSecret) {
                Config.BroadcasterClientSecret = BroadcasterClientSecretInput.Text!;
                Cache.ClearBroadcasterAuthorizationData();
                MainWindow.MainPanel.BroadcasterButton.UpdateState();
            }

            if (ChatterClientIdInput.Text != Config.ChatterClientId) {
                Config.ChatterClientId = ChatterClientIdInput.Text!;
                Cache.ClearChatterAuthorizationData();
                MainWindow.MainPanel.ChatterButton.UpdateState();
            }

            if (ChatterClientSecretInput.Text != Config.ChatterClientSecret) {
                Config.ChatterClientSecret = ChatterClientSecretInput.Text!;
                Cache.ClearChatterAuthorizationData();
                MainWindow.MainPanel.ChatterButton.UpdateState();
            }

            Config.AuthorizationPort = (int)AuthorizationPortInput.Value!;
            if ((int)NumMaxLogFilesInput.Value! != Config.NumMaxLogFiles) {
                Config.NumMaxLogFiles = (int)NumMaxLogFilesInput.Value;
                _ = Utils.FireTryElseError(Logger.DeleteExcessFiles, CancellationToken.None);
            }

            Config.WebSocketConnectTimeoutSeconds = (int)WebSocketConnectTimeoutSecondsInput.Value!;
            Config.WebSocketKeepaliveTimeoutSeconds = (int)WebSocketKeepaliveTimeoutSecondsInput.Value!;
            Config.WebSocketKeepaliveTimeoutMarginSeconds = (int)WebSocketKeepaliveTimeoutMarginSecondsInput.Value!;
            Utils.TryElseError(Config.Save);
            IsVisible = false;
            MainWindow.MainPanel.IsVisible = true;
        };

        private static SGrid GetBody(SGrid basicConfigGrid, STextBlock separator, SGrid advancedConfigGrid) => new([
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GridLength.Star,
            ], [
                basicConfigGrid,
                separator,
                advancedConfigGrid,
            ]);

        private static SGrid GetBasicConfigGrid(Controls children) {
            var basicConfigGrid = GetConfigGrid(children);
            basicConfigGrid.Margin = new(50d, 10d, 10d, 50d);
            return basicConfigGrid;
        }

        private static STextBlock GetBodySeparator() => new() {
            Text = "- Advanced -",
        };

        private static SGrid GetAdvancedConfigGrid(Controls children) {
            var advancedConfigGrid = GetConfigGrid(children);
            advancedConfigGrid.Margin = new(50d, 10d, 10d, 10d);
            return advancedConfigGrid;
        }

        private static SPopup GetAuthorizationPortPopup() => GetConfigValueInfoPopup("Authorization Port", [
                new Run("For full setup instructions, go to:\n"),
                GetUrlLinkInline("https://github.com/stone50/Stonebot"),
                new Run("\nThis is the localhost port used to authorize Stonebot. This should match the last portion of the OAuth Redirect URLs field of your Twitch's Stonebot applications, which can be found at:\n"),
                GetUrlLinkInline("https://dev.twitch.tv/console"),
            ], Constants.AuthorizationPortMin, Constants.AuthorizationPortMax, Constants.AuthorizationPortDefault);

        private static SPopup GetNumMaxLogFilesPopup() => GetConfigValueInfoPopup("Max Log Files", [
                new Run("Every time Stonebot is launched, it writes a new log file to "),
                GetFolderLinkInline("this folder in your local app data folder", Constants.LogsPath),
                new Run(". If the number of files in the logs folder exceeds this value, logs will be deleted, starting from the oldest."),
            ], Constants.NumMaxLogFilesMin, Constants.NumMaxLogFilesMax, Constants.NumMaxLogFilesDefault);

        private static SPopup GetWebSocketConnectTimeoutSecondsPopup() => GetConfigValueInfoPopup("Connect Timeout Seconds", [
                new Run("This is the number of seconds Stonebot will wait when trying to connect to Twitch before considering it a failed attempt."),
            ], Constants.WebSocketConnectTimeoutSecondsMin, Constants.WebSocketConnectTimeoutSecondsMax, Constants.WebSocketConnectTimeoutSecondsDefault);

        private static SPopup GetWebSocketKeepaliveTimeoutSecondsPopup() {
            var keepaliveMessageUrlLink = new UrlLink("https://dev.twitch.tv/docs/eventsub/handling-websocket-events/#keepalive-message");
            ((STextBlock)keepaliveMessageUrlLink.Content!).MaxWidth = 700d;
            return GetConfigValueInfoPopup("Keepalive Timeout Seconds", [
                new Run("This controls the frequency that Twitch sends a keepalive message when Stonebot is connected and no other messages are being sent. A higher value means less traffic when the broadcaster's chat is slow, but it may take longer to detect a lost connection. For more info, go to:"),
                keepaliveMessageUrlLink.GetInline(),
            ], Constants.WebSocketKeepaliveTimeoutSecondsMin, Constants.WebSocketKeepaliveTimeoutSecondsMax, Constants.WebSocketKeepaliveTimeoutSecondsDefault);
        }

        private static SPopup GetWebSocketKeepaliveTimeoutMarginSecondsPopup() {
            var keepaliveMessageUrlLink = new UrlLink("https://dev.twitch.tv/docs/eventsub/handling-websocket-events/#keepalive-message");
            ((STextBlock)keepaliveMessageUrlLink.Content!).MaxWidth = 700d;
            return GetConfigValueInfoPopup("Keepalive Timeout Margin Seconds", [
                new Run("This is the number of seconds Stonebot will wait after not receiving an expected keepalive message from Twitch before considering the connection lost. For info, go to:"),
                keepaliveMessageUrlLink.GetInline(),
            ], Constants.WebSocketKeepaliveTimeoutMarginSecondsMin, Constants.WebSocketKeepaliveTimeoutMarginSecondsMax, Constants.WebSocketKeepaliveTimeoutMarginSecondsDefault);
        }

        private static SGrid GetConfigGrid(Controls children) => new(
            [.. Enumerable.Repeat(GridLength.Auto, children.Count)],
            [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ],
            children
        );

        private static STextBox GetMaskedConfigValueTextBox() => new() {
            PasswordChar = '*',
            Width = 340d,
        };

        private static SNumericUpDown GetNumericUpDown(decimal min, decimal max, bool showSpinner) => new(min, max, showSpinner) {
            Width = 75d,
        };

        private static STextBlock GetConfigValueLabel(string text) => new() {
            Text = text,
            FontSize = 24d,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        private static InfoButton GetConfigValueInfoButton(SPopup popup) {
            var button = new InfoButton() {
                Content = "?",
                CornerRadius = new(20d),
                Padding = new(0d),
                Width = 30d,
                Height = 30d,
            };
            button.Click += (_, _) => popup.IsVisible = true;
            return button;
        }

        private static SPopup GetConfigValueInfoPopup(string label, InlineCollection inlines, int minValue, int maxValue, int defaultValue) => GetConfigValueInfoPopup(label, [
            ..inlines,
            new Run($"\nValid values: {minValue}-{maxValue} (default: {defaultValue})"),
        ]);

        private static SPopup GetConfigValueInfoPopup(string label, InlineCollection inlines) {
            var okButton = new InfoButton() {
                Content = "Ok",
                Margin = new(0d, 10d, 0d, 0d),
                MaxHeight = 50d,
            };
            var popup = new SPopup(label, inlines, okButton);
            okButton.Click += (_, _) => popup.IsVisible = false;
            return popup;
        }

        private static SPopup GetBasicConfigValueInfoPopup(string label, string valueText, string applicationTypeText) => GetConfigValueInfoPopup(label, [
            new Run("For full setup instructions, go to:\n"),
            GetUrlLinkInline("https://github.com/stone50/Stonebot"),
            new Run($"\nThis is the {valueText} of your Twitch's Stonebot {applicationTypeText} application, which can be found at:\n"),
            GetUrlLinkInline("https://dev.twitch.tv/console"),
        ]);

        private static InlineUIContainer GetUrlLinkInline(string url) => new UrlLink(url).GetInline();

        private static InlineUIContainer GetFolderLinkInline(string label, string path) => new FolderLink(label, path).GetInline();
    }
}
