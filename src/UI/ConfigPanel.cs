namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Buttons.Links;
    using CustomControls.Popups;
    using Helpers;

    internal class ConfigPanel : Panel {
        public ConfigPanel(Swappable swappableContent, MainPanel mainPanel) {
            Background = MainTheme.PrimaryBrush1;

            var configIcon = UIUtils.GetConfigIcon();
            var headerTitle = GetHeaderTitle();
            var cancelButton = GetCancelButton(swappableContent);
            var saveButton = GetSaveButton(swappableContent, mainPanel);
            var header = GetHeader(configIcon, headerTitle, cancelButton, saveButton);
            clientIdInput = new STextBox();
            var clientIdPopupOkButton = GetConfigValueInfoPopupOkButton();
            var clientIdPopup = GetClientIdPopup(clientIdPopupOkButton);
            numMaxLogFilesInput = GetNumericUpDown(Constants.NumMaxLogFilesMin, Constants.NumMaxLogFilesMax, true);
            var numMaxLogFilesPopupOkButton = GetConfigValueInfoPopupOkButton();
            var numMaxLogFilesPopup = GetNumMaxLogFilesPopup(numMaxLogFilesPopupOkButton);
            var configGrid = GetConfigGrid([
                GetConfigValueLabel("Client ID"),
                GetConfigValueInfoButton(clientIdPopup),
                clientIdInput,
                GetConfigValueLabel("Max Log Files"),
                GetConfigValueInfoButton(numMaxLogFilesPopup),
                numMaxLogFilesInput,
            ]);
            var body = GetBody(configGrid);
            var mainGrid = GetMainGrid(header, body);
            Children.Add(mainGrid);
            Children.Add(clientIdPopup);
            Children.Add(numMaxLogFilesPopup);
        }

        public void Update() {
            clientIdInput.Text = Config.ClientId;
            numMaxLogFilesInput.Value = Config.NumMaxLogFiles;
        }

        private readonly STextBox clientIdInput;
        private readonly SNumericUpDown numMaxLogFilesInput;

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
            if (clientIdInput.Text != Config.ClientId) {
                Config.ClientId = clientIdInput.Text!;
                mainPanel.UpdateBroadcasterButton();
            }

            if ((int)numMaxLogFilesInput.Value! != Config.NumMaxLogFiles) {
                Config.NumMaxLogFiles = (int)numMaxLogFilesInput.Value;
                _ = TaskHelper.FireTryElseError(Logger.DeleteExcessFiles);
            }

            ExceptionHelper.TryElseError(Config.Save);
            swappableContent.Swap();
        };

        private static ScrollViewer GetBody(SGrid configGrid) => new() {
            Content = configGrid,
        };

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

        private static SPopup GetClientIdPopup(InfoButton okButton) => GetConfigValueInfoPopup("Client ID", [
            new Run("For full setup instructions, go to:\n"),
            GetUrlLinkInline("https://github.com/stone50/Stonebot"),
            new Run($"\nThis is the client ID of your Twitch application, which can be found at:\n"),
            GetUrlLinkInline("https://dev.twitch.tv/console"),
        ], okButton);

        private static SPopup GetNumMaxLogFilesPopup(InfoButton okButton) => GetConfigValueInfoPopup("Max Log Files", [
            new Run("Every time Stonebot is launched, it writes a new log file to "),
            GetFolderLinkInline("this folder in your local app data folder", Constants.LogsPath),
            new Run(". If the number of files in the logs folder exceeds this value, logs will be deleted, starting from the oldest."),
        ], Constants.NumMaxLogFilesMin, Constants.NumMaxLogFilesMax, Constants.NumMaxLogFilesDefault, okButton);

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

        private static InlineUIContainer GetUrlLinkInline(string url) => new UrlLink(url).GetInline();

        private static InlineUIContainer GetFolderLinkInline(string label, string path) => new FolderLink(label, path).GetInline();
    }
}
