namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Media;
    using Stonebot.UI.Buttons;

    internal class ConfigPanel : Panel {
        public readonly MainWindow MainWindow;
        public readonly TextBox BroadcasterClientIdInput;
        public readonly TextBox BroadcasterClientSecretInput;
        public readonly TextBox ChatterClientIdInput;
        public readonly TextBox ChatterClientSecretInput;
        public readonly NumericUpDown AuthorizationPortInput;
        public readonly NumericUpDown NumMaxLogFilesInput;

        public ConfigPanel(MainWindow mainWindow) {
            MainWindow = mainWindow;
            var cancelButton = GetExitButton("Cancel", MainTheme.DangerBrush2, MainTheme.DangerBrush1, MainTheme.DangerBrush3);
            cancelButton.Click += (_, _) => {
                IsVisible = false;
                MainWindow.MainPanel.IsVisible = true;
            };
            var saveButton = GetExitButton("Save", MainTheme.SuccessBrush2, MainTheme.SuccessBrush1, MainTheme.SuccessBrush3);
            saveButton.Click += (_, _) => Save();
            var header = new Grid {
                Background = MainTheme.PrimaryBrush1,
                ColumnDefinitions = [
                    new(GridLength.Star),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                Children = {
                    new TextBlock {
                        Text = "Config",
                        FontFamily = MainTheme.Font,
                        FontSize = 48d,
                        Foreground = MainTheme.NeutralBrush1,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new(10d),
                    },
                    cancelButton,
                    saveButton,
                },
                Height = 100d,
            };
            for (var i = 0; i < header.Children.Count; ++i) {
                Grid.SetColumn(header.Children[i], i);
            }

            BroadcasterClientIdInput = GetConfigValueTextBox();
            BroadcasterClientSecretInput = GetConfigValueTextBox();
            ChatterClientIdInput = GetConfigValueTextBox();
            ChatterClientSecretInput = GetConfigValueTextBox();
            AuthorizationPortInput = new() {
                Minimum = 1024M,
                Maximum = 49151M,
                ParsingNumberStyle = System.Globalization.NumberStyles.Integer,
                AllowSpin = false,
                ShowButtonSpinner = false,
                FontFamily = MainTheme.Font,
                FontSize = 18d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 75d,
            };
            NumMaxLogFilesInput = new() {
                Minimum = 1M,
                ParsingNumberStyle = System.Globalization.NumberStyles.Integer,
                FontFamily = MainTheme.Font,
                FontSize = 18d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 75d,
            };
            // TODO: add tooltips to inform the user of what the config value means
            var body = new Grid {
                RowDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                ColumnDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                RowSpacing = 20d,
                ColumnSpacing = 10d,
                Children = {
                    GetConfigValueLabel("Broadcaster Client ID"),
                    BroadcasterClientIdInput,
                    GetConfigValueLabel("Broadcaster Client Secret"),
                    BroadcasterClientSecretInput,
                    GetConfigValueLabel("Chatter Client ID"),
                    ChatterClientIdInput,
                    GetConfigValueLabel("Chatter Client Secret"),
                    ChatterClientSecretInput,
                    GetConfigValueLabel("Authorization Port"),
                    AuthorizationPortInput,
                    GetConfigValueLabel("Max Log Files"),
                    NumMaxLogFilesInput,
                },
                Margin = new(10d),
            };
            for (var i = 0; i < body.Children.Count; ++i) {
                Grid.SetColumn(body.Children[i], i % 2);
                Grid.SetRow(body.Children[i], i / 2);
            }

            var fullGrid = new Grid {
                RowDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                Children = {
                    header,
                    body,
                }
            };
            for (var i = 0; i < fullGrid.Children.Count; ++i) {
                Grid.SetRow(fullGrid.Children[i], i);
            }

            Background = MainTheme.PrimaryBrush2;
            Children.Add(fullGrid);
        }

        public void Show() {
            BroadcasterClientIdInput.Text = Config.BroadcasterClientId;
            BroadcasterClientSecretInput.Text = Config.BroadcasterClientSecret;
            ChatterClientIdInput.Text = Config.ChatterClientId;
            ChatterClientSecretInput.Text = Config.ChatterClientSecret;
            AuthorizationPortInput.Value = Config.AuthorizationPort;
            NumMaxLogFilesInput.Value = Config.NumMaxLogFiles;
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

        private static SButton GetExitButton(
            string content,
            IImmutableBrush defaultBrush,
            IImmutableBrush hoveredBrush,
            IImmutableBrush pressedBrush
        ) => new(defaultBrush, hoveredBrush, pressedBrush) {
            Content = content,
            FontFamily = MainTheme.Font,
            FontSize = 18d,
            Foreground = MainTheme.NeutralBrush1,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center,
            CornerRadius = new(5d),
            Padding = new(10d),
            Margin = new(30d, 0d),
            Height = 50d
        };

        private static TextBox GetConfigValueTextBox() => new() {
            PasswordChar = '*',
            FontFamily = MainTheme.Font,
            FontSize = 18d,
            Foreground = MainTheme.NeutralBrush1,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 340d,
        };

        private static TextBlock GetConfigValueLabel(string text) => new() {
            Text = text,
            FontFamily = MainTheme.Font,
            FontSize = 24d,
            Foreground = MainTheme.NeutralBrush1,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };
    }
}
