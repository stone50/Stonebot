namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Buttons.Links;
    using CustomControls.Popups;

    internal class ConfigPanel : Panel {
        public ConfigPanel(Swappable swappableContent, MainPanel mainPanel) {
            Background = MainTheme.PrimaryBrush1;

            var configIcon = UIUtils.GetConfigIcon();
            var headerTitle = GetHeaderTitle();
            var cancelButton = GetCancelButton(swappableContent);
            var saveButton = GetSaveButton(swappableContent, mainPanel);
            var header = GetHeader(configIcon, headerTitle, cancelButton, saveButton);
            broadcasterClientIdInput = GetMaskedConfigValueTextBox();
            var broadcasterClientIdPopupOkButton = GetConfigValueInfoPopupOkButton();
            var broadcasterClientIdPopup = GetBasicConfigValueInfoPopup("Broadcaster Client ID", "client ID", "broadcaster", broadcasterClientIdPopupOkButton);
            broadcasterClientSecretInput = GetMaskedConfigValueTextBox();
            var broadcasterClientSecretPopupOkButon = GetConfigValueInfoPopupOkButton();
            var broadcasterClientSecretPopup = GetBasicConfigValueInfoPopup("Broadcaster Client Secret", "client secret", "broadcaster", broadcasterClientSecretPopupOkButon);
            chatterClientIdInput = GetMaskedConfigValueTextBox();
            var chatterClientIdPopupOkButton = GetConfigValueInfoPopupOkButton();
            var chatterClientIdPopup = GetBasicConfigValueInfoPopup("Chatter Client ID", "client ID", "chatter", chatterClientIdPopupOkButton);
            chatterClientSecretInput = GetMaskedConfigValueTextBox();
            var chatterClientSecretPopupOkButton = GetConfigValueInfoPopupOkButton();
            var chatterClientSecretPopup = GetBasicConfigValueInfoPopup("Chatter Client Secret", "client secret", "chatter", chatterClientSecretPopupOkButton);
            var basicConfigGrid = GetConfigGrid([
                GetConfigValueLabel("Broadcaster Client ID"),
                GetConfigValueInfoButton(broadcasterClientIdPopup),
                broadcasterClientIdInput,
                GetConfigValueLabel("Broadcaster Client Secret"),
                GetConfigValueInfoButton(broadcasterClientSecretPopup),
                broadcasterClientSecretInput,
                GetConfigValueLabel("Chatter Client ID"),
                GetConfigValueInfoButton(chatterClientIdPopup),
                chatterClientIdInput,
                GetConfigValueLabel("Chatter Client Secret"),
                GetConfigValueInfoButton(chatterClientSecretPopup),
                chatterClientSecretInput,
            ]);
            var bodySeparatorText = GetBodySeparatorText();
            var bodySeparator = GetBodySeparator(bodySeparatorText);
            var authorizationPortPopupOkButton = GetConfigValueInfoPopupOkButton();
            var authorizationPortPopup = GetAuthorizationPortPopup(authorizationPortPopupOkButton);
            authorizationPortInput = GetNumericUpDown(Constants.AuthorizationPortMin, Constants.AuthorizationPortMax, false);
            var numMaxLogFilesPopupOkButton = GetConfigValueInfoPopupOkButton();
            var numMaxLogFilesPopup = GetNumMaxLogFilesPopup(numMaxLogFilesPopupOkButton);
            numMaxLogFilesInput = GetNumericUpDown(Constants.NumMaxLogFilesMin, Constants.NumMaxLogFilesMax, true);
            var webSocketConnectTimeoutSecondsPopupOkButton = GetConfigValueInfoPopupOkButton();
            var webSocketConnectTimeoutSecondsPopup = GetWebSocketConnectTimeoutSecondsPopup(webSocketConnectTimeoutSecondsPopupOkButton);
            webSocketConnectTimeoutSecondsInput = GetNumericUpDown(Constants.WebSocketConnectTimeoutSecondsMin, Constants.WebSocketConnectTimeoutSecondsMax, true);
            var webSocketKeepaliveTimeoutSecondsPopupOkButton = GetConfigValueInfoPopupOkButton();
            var webSocketKeepaliveTimeoutSecondsPopup = GetWebSocketKeepaliveTimeoutSecondsPopup(webSocketKeepaliveTimeoutSecondsPopupOkButton);
            webSocketKeepaliveTimeoutSecondsInput = GetNumericUpDown(Constants.WebSocketKeepaliveTimeoutSecondsMin, Constants.WebSocketKeepaliveTimeoutSecondsMax, true);
            var webSocketKeepaliveTimeoutMarginSecondsPopupOkButton = GetConfigValueInfoPopupOkButton();
            var webSocketKeepaliveTimeoutMarginSecondsPopup = GetWebSocketKeepaliveTimeoutMarginSecondsPopup(webSocketKeepaliveTimeoutMarginSecondsPopupOkButton);
            webSocketKeepaliveTimeoutMarginSecondsInput = GetNumericUpDown(Constants.WebSocketKeepaliveTimeoutMarginSecondsMin, Constants.WebSocketKeepaliveTimeoutMarginSecondsMax, true);
            var advancedConfigGrid = GetConfigGrid([
                GetConfigValueLabel("Authorization Port"),
                GetConfigValueInfoButton(authorizationPortPopup),
                authorizationPortInput,
                GetConfigValueLabel("Max Log Files"),
                GetConfigValueInfoButton(numMaxLogFilesPopup),
                numMaxLogFilesInput,
                GetConfigValueLabel("Connect Timeout Seconds"),
                GetConfigValueInfoButton(webSocketConnectTimeoutSecondsPopup),
                webSocketConnectTimeoutSecondsInput,
                GetConfigValueLabel("Keepalive Timeout Seconds"),
                GetConfigValueInfoButton(webSocketKeepaliveTimeoutSecondsPopup),
                webSocketKeepaliveTimeoutSecondsInput,
                GetConfigValueLabel("Keepalive Timeout Margin Seconds"),
                GetConfigValueInfoButton(webSocketKeepaliveTimeoutMarginSecondsPopup),
                webSocketKeepaliveTimeoutMarginSecondsInput,
            ]);
            var bodyGrid = GetBodyGrid(basicConfigGrid, bodySeparator, advancedConfigGrid);
            var body = GetBody(bodyGrid);
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

        public void Init() {
            broadcasterClientIdInput.Text = Config.BroadcasterClientId;
            broadcasterClientSecretInput.Text = Config.BroadcasterClientSecret;
            chatterClientIdInput.Text = Config.ChatterClientId;
            chatterClientSecretInput.Text = Config.ChatterClientSecret;
            authorizationPortInput.Value = Config.AuthorizationPort;
            numMaxLogFilesInput.Value = Config.NumMaxLogFiles;
            webSocketConnectTimeoutSecondsInput.Value = Config.WebSocketConnectTimeoutSeconds;
            webSocketKeepaliveTimeoutSecondsInput.Value = Config.WebSocketKeepaliveTimeoutSeconds;
            webSocketKeepaliveTimeoutMarginSecondsInput.Value = Config.WebSocketKeepaliveTimeoutMarginSeconds;
        }

        private readonly STextBox broadcasterClientIdInput;
        private readonly STextBox broadcasterClientSecretInput;
        private readonly STextBox chatterClientIdInput;
        private readonly STextBox chatterClientSecretInput;
        private readonly SNumericUpDown authorizationPortInput;
        private readonly SNumericUpDown numMaxLogFilesInput;
        private readonly SNumericUpDown webSocketConnectTimeoutSecondsInput;
        private readonly SNumericUpDown webSocketKeepaliveTimeoutSecondsInput;
        private readonly SNumericUpDown webSocketKeepaliveTimeoutMarginSecondsInput;

        private static SGrid GetMainGrid(SGrid header, ScrollViewer body) => new([
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
            Background = MainTheme.PrimaryBrush2,
            Height = 150d,
        };

        private static STextBlock GetHeaderTitle() => new() {
            FontSize = 48d,
            HorizontalAlignment = HorizontalAlignment.Left,
            Text = "Config",
        };

        private static DangerButton GetCancelButton(Swappable swappableContent) {
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                VerticalAlignment = VerticalAlignment.Center,
            };
            cancelButton.Click += (_, _) => swappableContent.Swap();
            return cancelButton;
        }

        private SuccessButton GetSaveButton(Swappable swappableContent, MainPanel mainPanel) {
            var saveButton = new SuccessButton() {
                Content = "Save",
                VerticalAlignment = VerticalAlignment.Center,
            };
            saveButton.Click += GetOnSaveButtonClick(swappableContent, mainPanel);
            return saveButton;
        }

        private EventHandler<RoutedEventArgs> GetOnSaveButtonClick(Swappable swappableContent, MainPanel mainPanel) => (_, _) => {
            if (broadcasterClientIdInput.Text != Config.BroadcasterClientId) {
                Config.BroadcasterClientId = broadcasterClientIdInput.Text!;
                Cache.ClearBroadcasterAuthorizationData();
                mainPanel.UpdateBroadcasterButton();
            }

            if (broadcasterClientSecretInput.Text != Config.BroadcasterClientSecret) {
                Config.BroadcasterClientSecret = broadcasterClientSecretInput.Text!;
                Cache.ClearBroadcasterAuthorizationData();
                mainPanel.UpdateBroadcasterButton();
            }

            if (chatterClientIdInput.Text != Config.ChatterClientId) {
                Config.ChatterClientId = chatterClientIdInput.Text!;
                Cache.ClearChatterAuthorizationData();
                mainPanel.UpdateChatterButton();
            }

            if (chatterClientSecretInput.Text != Config.ChatterClientSecret) {
                Config.ChatterClientSecret = chatterClientSecretInput.Text!;
                Cache.ClearChatterAuthorizationData();
                mainPanel.UpdateChatterButton();
            }

            Config.AuthorizationPort = (int)authorizationPortInput.Value!;
            if ((int)numMaxLogFilesInput.Value! != Config.NumMaxLogFiles) {
                Config.NumMaxLogFiles = (int)numMaxLogFilesInput.Value;
                _ = Utils.FireTryElseError(Logger.DeleteExcessFiles, CancellationToken.None);
            }

            Config.WebSocketConnectTimeoutSeconds = (int)webSocketConnectTimeoutSecondsInput.Value!;
            Config.WebSocketKeepaliveTimeoutSeconds = (int)webSocketKeepaliveTimeoutSecondsInput.Value!;
            Config.WebSocketKeepaliveTimeoutMarginSeconds = (int)webSocketKeepaliveTimeoutMarginSecondsInput.Value!;
            Utils.TryElseError(Config.Save);
            swappableContent.Swap();
        };

        private static ScrollViewer GetBody(SGrid bodyGrid) => new() {
            Content = bodyGrid,
        };

        private static SGrid GetBodyGrid(SGrid basicConfigGrid, Border separator, SGrid advancedConfigGrid) => new([
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

        private static Border GetBodySeparator(STextBlock bodySeparatorText) => new() {
            Background = MainTheme.PrimaryBrush2,
            Child = bodySeparatorText,
            CornerRadius = new(5d),
        };

        private static STextBlock GetBodySeparatorText() => new() {
            FontSize = 24d,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            Text = "- Advanced -",
        };

        private static SPopup GetAuthorizationPortPopup(InfoButton okButton) => GetConfigValueInfoPopup("Authorization Port", [
                new Run("For full setup instructions, go to:\n"),
                GetUrlLinkInline("https://github.com/stone50/Stonebot"),
                new Run("\nThis is the localhost port used to authorize Stonebot. This should match the last portion of the OAuth Redirect URLs field of your Twitch's Stonebot applications, which can be found at:\n"),
                GetUrlLinkInline("https://dev.twitch.tv/console"),
            ], Constants.AuthorizationPortMin, Constants.AuthorizationPortMax, Constants.AuthorizationPortDefault, okButton);

        private static SPopup GetNumMaxLogFilesPopup(InfoButton okButton) => GetConfigValueInfoPopup("Max Log Files", [
                new Run("Every time Stonebot is launched, it writes a new log file to "),
                GetFolderLinkInline("this folder in your local app data folder", Constants.LogsPath),
                new Run(". If the number of files in the logs folder exceeds this value, logs will be deleted, starting from the oldest."),
            ], Constants.NumMaxLogFilesMin, Constants.NumMaxLogFilesMax, Constants.NumMaxLogFilesDefault, okButton);

        private static SPopup GetWebSocketConnectTimeoutSecondsPopup(InfoButton okButton) => GetConfigValueInfoPopup("Connect Timeout Seconds", [
                new Run("This is the number of seconds Stonebot will wait when trying to connect to Twitch before considering it a failed attempt."),
            ], Constants.WebSocketConnectTimeoutSecondsMin, Constants.WebSocketConnectTimeoutSecondsMax, Constants.WebSocketConnectTimeoutSecondsDefault, okButton);

        private static SPopup GetWebSocketKeepaliveTimeoutSecondsPopup(InfoButton okButton) {
            var keepaliveMessageUrlLink = new UrlLink("https://dev.twitch.tv/docs/eventsub/handling-websocket-events/#keepalive-message");
            ((STextBlock)keepaliveMessageUrlLink.Content!).MaxWidth = 700d;
            return GetConfigValueInfoPopup("Keepalive Timeout Seconds", [
                new Run("This controls the frequency that Twitch sends a keepalive message when Stonebot is connected and no other messages are being sent. A higher value means less traffic when the broadcaster's chat is slow, but it may take longer to detect a lost connection. For more info, go to:"),
                keepaliveMessageUrlLink.GetInline(),
            ], Constants.WebSocketKeepaliveTimeoutSecondsMin, Constants.WebSocketKeepaliveTimeoutSecondsMax, Constants.WebSocketKeepaliveTimeoutSecondsDefault, okButton);
        }

        private static SPopup GetWebSocketKeepaliveTimeoutMarginSecondsPopup(InfoButton okButton) {
            var keepaliveMessageUrlLink = new UrlLink("https://dev.twitch.tv/docs/eventsub/handling-websocket-events/#keepalive-message");
            ((STextBlock)keepaliveMessageUrlLink.Content!).MaxWidth = 700d;
            return GetConfigValueInfoPopup("Keepalive Timeout Margin Seconds", [
                new Run("This is the number of seconds Stonebot will wait after not receiving an expected keepalive message from Twitch before considering the connection lost. For info, go to:"),
                keepaliveMessageUrlLink.GetInline(),
            ], Constants.WebSocketKeepaliveTimeoutMarginSecondsMin, Constants.WebSocketKeepaliveTimeoutMarginSecondsMax, Constants.WebSocketKeepaliveTimeoutMarginSecondsDefault, okButton);
        }

        private static SGrid GetConfigGrid(Controls children) => new(
            [.. Enumerable.Repeat(GridLength.Auto, children.Count)],
            [
                GridLength.Auto,
                GridLength.Auto,
                GridLength.Auto,
            ],
            children
        ) {
            Margin = new(20d, 10d),
        };

        private static STextBox GetMaskedConfigValueTextBox() => new() {
            PasswordChar = '*',
            Width = 340d,
        };

        private static SNumericUpDown GetNumericUpDown(decimal min, decimal max, bool showSpinner) => new(min, max, showSpinner) {
            Width = 75d,
        };

        private static STextBlock GetConfigValueLabel(string text) => new() {
            HorizontalAlignment = HorizontalAlignment.Left,
            Text = text,
        };

        private static InfoButton GetConfigValueInfoButton(SPopup popup) {
            var button = new InfoButton() {
                Content = "?",
                CornerRadius = new(20d),
                Height = 30d,
                Padding = new(0d),
                Width = 30d,
            };
            button.Click += (_, _) => popup.IsVisible = true;
            return button;
        }

        private static SPopup GetConfigValueInfoPopup(string label, InlineCollection inlines, int minValue, int maxValue, int defaultValue, InfoButton okButton) => GetConfigValueInfoPopup(label, [
            ..inlines,
            new Run($"\nValid values: {minValue}-{maxValue} (default: {defaultValue})"),
        ], okButton);

        private static SPopup GetConfigValueInfoPopup(string label, InlineCollection inlines, InfoButton okButton) {
            var popup = new SPopup(label, inlines, okButton);
            okButton.Click += (_, _) => popup.IsVisible = false;
            return popup;
        }

        private static InfoButton GetConfigValueInfoPopupOkButton() => new() {
            Content = "Ok",
            Margin = new(0d, 10d, 0d, 0d),
            MaxHeight = 50d,
        };

        private static SPopup GetBasicConfigValueInfoPopup(string label, string valueText, string applicationTypeText, InfoButton okButton) => GetConfigValueInfoPopup(label, [
            new Run("For full setup instructions, go to:\n"),
            GetUrlLinkInline("https://github.com/stone50/Stonebot"),
            new Run($"\nThis is the {valueText} of your Twitch's Stonebot {applicationTypeText} application, which can be found at:\n"),
            GetUrlLinkInline("https://dev.twitch.tv/console"),
        ], okButton);

        private static InlineUIContainer GetUrlLinkInline(string url) => new UrlLink(url).GetInline();

        private static InlineUIContainer GetFolderLinkInline(string label, string path) => new FolderLink(label, path).GetInline();
    }
}
