namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Buttons;
    using System;

    internal class MainPanel : Panel {
        public readonly MainWindow MainWindow;
        public readonly ConnectButton ConnectButton;
        public readonly BroadcasterButton BroadcasterButton;
        public readonly ChatterButton ChatterButton;
        public readonly AuthorizePopup AuthorizePopup;
        public readonly RemoveAuthorizationPopup RemoveAuthorizationPopup;

        public MainPanel(MainWindow mainWindow) {
            MainWindow = mainWindow;
            ConnectButton = new(this) {
                FontFamily = MainTheme.Font,
                FontSize = 18d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(50d),
                Margin = new(10d),
                Padding = new(15d, 5d, 15d, 5d),
                MaxHeight = 50d,
                MinWidth = 220d,
            };
            RemoveAuthorizationPopup = new(this);
            AuthorizePopup = new(this);
            BroadcasterButton = new(this);
            ChatterButton = new(this);
            var users = new Grid {
                RowDefinitions = [
                    new(GridLength.Star),
                    new(GridLength.Star),
                ],
                ColumnDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                RowSpacing = 10d,
                ColumnSpacing = 10d,
                VerticalAlignment = VerticalAlignment.Center,
                Children = {
                    GetUserLabel("Broadcaster:"),
                    BroadcasterButton,
                    GetUserLabel("Chatter:"),
                    ChatterButton,
                },
                Margin = new(10d),
            };
            for (var i = 0; i < users.Children.Count; ++i) {
                Grid.SetColumn(users.Children[i], i % 2);
                Grid.SetRow(users.Children[i], i / 2);
            }

            // TODO: make this a cog icon
            var configButton = new SButton(
                MainTheme.InfoBrush3,
                MainTheme.InfoBrush1,
                MainTheme.InfoBrush2
            ) {
                Content = "Config",
                FontFamily = MainTheme.Font,
                FontSize = 18d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(5d),
                Padding = new(15d, 10d),
                Margin = new(10d),
                Height = 50d,
            };
            configButton.Click += (_, _) => {
                IsVisible = false;
                MainWindow.ConfigPanel.Show();
            };
            var header = new Grid {
                Background = MainTheme.PrimaryBrush1,
                ColumnDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Star),
                    new(GridLength.Auto),
                ],
                Children = {
                    GetLogo(),
                    ConnectButton,
                    users,
                    configButton,
                },
                Height = 150d,
            };
            for (var i = 0; i < header.Children.Count; ++i) {
                Grid.SetColumn(header.Children[i], i);
            }

            var body = new Grid();
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
            Children.Add(RemoveAuthorizationPopup);
            Children.Add(AuthorizePopup);
        }

        public void UpdateUsers() {
            BroadcasterButton.UpdateState();
            ChatterButton.UpdateState();
        }

        private static TextBlock GetUserLabel(string text) => new() {
            Text = text,
            FontFamily = MainTheme.Font,
            FontSize = 18d,
            Foreground = MainTheme.NeutralBrush1,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
        };

        private static Image GetLogo() {
            var uri = new Uri("avares://Stonebot/Assets/logo.png");
            var assetStream = AssetLoader.Open(uri);
            var bitmap = new Bitmap(assetStream);
            var logo = new Image {
                Source = bitmap,
                Stretch = Stretch.UniformToFill,
                Margin = new(10d),
            };
            RenderOptions.SetBitmapInterpolationMode(logo, BitmapInterpolationMode.None);
            return logo;
        }
    }
}
