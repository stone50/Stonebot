namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using Buttons;
    using Popups;
    using Stonebot.UI.Buttons.Links;

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

        public ConfigPanel(MainWindow mainWindow) {
            MainWindow = mainWindow;
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                Height = 60d
            };
            cancelButton.Click += (_, _) => {
                IsVisible = false;
                MainWindow.MainPanel.IsVisible = true;
            };
            var saveButton = new SuccessButton() {
                Content = "Save",
                Height = 60d
            };
            saveButton.Click += (_, _) => Save();
            var configIcon = UIUtils.GetConfigIcon();
            var header = new SGrid([
                GridLength.Star
            ], [
                GridLength.Auto,
                GridLength.Star,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                configIcon,
                new STextBlock {
                    Text = "Config",
                    FontSize = 48d,
                    HorizontalAlignment = HorizontalAlignment.Left,
                },
                cancelButton,
                saveButton,
            ]) {
                Background = MainTheme.PrimaryBrush1,
                Height = 150d,
            };
            BroadcasterClientIdInput = GetMaskedConfigValueTextBox();
            BroadcasterClientSecretInput = GetMaskedConfigValueTextBox();
            ChatterClientIdInput = GetMaskedConfigValueTextBox();
            ChatterClientSecretInput = GetMaskedConfigValueTextBox();
            var broadcasterClientIdPopup = GetBasicConfigValueInfoPopup("Broadcaster Client ID", "client ID", "broadcaster");
            var broadcasterClientSecretPopup = GetBasicConfigValueInfoPopup("Broadcaster Client Secret", "client secret", "broadcaster");
            var chatterClientIdPopup = GetBasicConfigValueInfoPopup("Chatter Client ID", "client ID", "chatter");
            var chatterClientSecretPopup = GetBasicConfigValueInfoPopup("Chatter Client Secret", "client secret", "chatter");
            var basicConfigGrid = new SGrid([
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
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
            AuthorizationPortInput = GetNumericUpDown(1024M, 49151M, false);
            AuthorizationPortInput.Width = 75d;
            NumMaxLogFilesInput = GetNumericUpDown(1M, int.MaxValue, true);
            NumMaxLogFilesInput.MinWidth = 75d;
            WebSocketConnectTimeoutSecondsInput = GetNumericUpDown(1M, int.MaxValue, true);
            WebSocketConnectTimeoutSecondsInput.MinWidth = 75d;
            WebSocketKeepaliveTimeoutSecondsInput = GetNumericUpDown(10M, 600M, true);
            WebSocketKeepaliveTimeoutSecondsInput.MinWidth = 75d;
            var authorizationPortPopup = GetConfigValueInfoPopup("Authorization Port", [
               new Run("For full setup instructions, go to:\n"),
                GetUrlLinkInline("https://github.com/stone50/Stonebot"),
                new Run("\nThis is the localhost port used to authorize Stonebot. This should match the last portion of the OAuth Redirect URLs field of your Twitch's Stonebot applications, which can be found at:\n"),
                GetUrlLinkInline("https://dev.twitch.tv/console"),
            ]);
            var numMaxLogFilesPopup = GetConfigValueInfoPopup("Max Log Files", [
                new Run("Every time Stonebot is launched, it writes a new log file to "),
                GetFolderLinkInline("this folder in your local app data folder", Constants.LogsPath),
                new Run(". If the number of files in the logs folder exceeds this value, logs will be deleted, starting from the oldest."),
            ]);
            var webSocketConnectTimeoutSecondsPopup = GetConfigValueInfoPopup("Web Socket Connect Timeout Seconds", [
                new Run("This is the number of seconds Stonebot will wait when trying to connect to Twitch before considering it a failed attempt."),
            ]);
            var keepaliveMessageUrlLink = new UrlLink("https://dev.twitch.tv/docs/eventsub/handling-websocket-events/#keepalive-message");
            ((STextBlock)keepaliveMessageUrlLink.Content!).MaxWidth = 700d;
            var webSocketKeepaliveTimeoutSecondsPopup = GetConfigValueInfoPopup("Web Socket Keepalive Timeout Seconds", [
                new Run("This controls the frequency that Twitch sends a keepalive message when Stonebot is connected and no other messages are being sent. A higher value means less traffic when the broadcaster's chat is slow, but it may take longer to detect a lost connection. For more info, go to:"),
                keepaliveMessageUrlLink.GetInline(),
            ]);
            var advancedConfigGrid = new SGrid([
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GetConfigValueLabel("Authorization Port"),
                GetConfigValueInfoButton(authorizationPortPopup),
                AuthorizationPortInput,
                GetConfigValueLabel("Max Log Files"),
                GetConfigValueInfoButton(numMaxLogFilesPopup),
                NumMaxLogFilesInput,
                GetConfigValueLabel("Web Socket Connect Timeout Seconds"),
                GetConfigValueInfoButton(webSocketConnectTimeoutSecondsPopup),
                WebSocketConnectTimeoutSecondsInput,
                GetConfigValueLabel("Web Socket Keepalive Timeout Seconds"),
                GetConfigValueInfoButton(webSocketKeepaliveTimeoutSecondsPopup),
                WebSocketKeepaliveTimeoutSecondsInput,
            ]);
            var body = new SGrid([
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ], [
                GridLength.Star,
            ], [
                basicConfigGrid,
                new STextBlock(){

                },
                advancedConfigGrid,
            ]);
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
            Children.Add(broadcasterClientIdPopup);
            Children.Add(broadcasterClientSecretPopup);
            Children.Add(chatterClientIdPopup);
            Children.Add(chatterClientSecretPopup);
            Children.Add(authorizationPortPopup);
            Children.Add(numMaxLogFilesPopup);
            Children.Add(webSocketConnectTimeoutSecondsPopup);
            Children.Add(webSocketKeepaliveTimeoutSecondsPopup);
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
            IsVisible = true;
        }

        private void Save() {
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
                FireDeleteExcessLogFiles();
            }

            Utils.TryElseError(Config.Save);
            IsVisible = false;
            MainWindow.MainPanel.IsVisible = true;
        }

        private static void FireDeleteExcessLogFiles() => Task.Run(() => Utils.TryElseError(Logger.DeleteExcessFiles));

        private static TextBox GetMaskedConfigValueTextBox() => new() {
            PasswordChar = '*',
            FontFamily = MainTheme.Font,
            Foreground = MainTheme.NeutralBrush1,
            FontSize = 18d,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 340d,
        };

        private static NumericUpDown GetNumericUpDown(decimal min, decimal max, bool showSpinner) => new() {
            Minimum = min,
            Maximum = max,
            Increment = 1M,
            ParsingNumberStyle = System.Globalization.NumberStyles.Integer,
            AllowSpin = showSpinner,
            ShowButtonSpinner = showSpinner,
            FontFamily = MainTheme.Font,
            FontSize = 18d,
            Foreground = MainTheme.NeutralBrush1,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };

        private static STextBlock GetConfigValueLabel(string text) => new() {
            Text = text,
            FontSize = 24d,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        private static InfoButton GetConfigValueInfoButton(SPopup popup) {
            var button = new InfoButton() {
                Content = "?",
                Padding = new(0d),
                Width = 30d,
                Height = 30d,
            };
            button.Click += (_, _) => popup.IsVisible = true;
            return button;
        }

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
