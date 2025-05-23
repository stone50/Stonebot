namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;

    internal class MainWindow : Window {


        public MainWindow() {
            Title = "Stonebot";
            Width = 1000;
            Height = 800;

            connectButton = new() {
                FontFamily = MainTheme.RobotoFont,
                FontSize = 18,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(50d),
                Margin = new(10),
                Padding = new(15, 5, 15, 8),
                MaxHeight = 50f,
            };

            broadcasterButton = GetUserButton();
            chatterButton = GetUserButton();
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
                    GetUserLabel("Broadcaster :"),
                    broadcasterButton,
                    GetUserLabel("Chatter :"),
                    chatterButton,
                },
                Margin = new(10),
            };
            for (var i = 0; i < users.Children.Count; ++i) {
                Grid.SetColumn(users.Children[i], i % 2);
                Grid.SetRow(users.Children[i], i / 2);
            }

            var header = new Grid {
                Background = MainTheme.PrimaryBrush1,
                ColumnDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                Children = {
                    GetLogo(),
                    connectButton,
                    users,
                },
                MinHeight = 150d,
                MaxHeight = 150d,
            };
            for (var i = 0; i < header.Children.Count; ++i) {
                Grid.SetColumn(header.Children[i], i);
            }

            var body = new Grid();
            Background = MainTheme.PrimaryBrush2;
            Content = new Grid {
                RowDefinitions = [
                    new(GridLength.Auto),
                ],
                Children = {
                    header,
                    body,
                }
            };
        }

        public void UpdateUsers() {
            broadcasterButton.Content = Cache.BroadcasterAuthorizationData is null ? "Click to Authorize" : Cache.BroadcasterAuthorizationData.UserLogin;
            chatterButton.Content = Cache.ChatterAuthorizationData is null ? "Click to Authorize" : Cache.ChatterAuthorizationData.UserLogin;
        }

        private readonly ConnectButton connectButton;
        private readonly SButton broadcasterButton;
        private readonly SButton chatterButton;

        private static SButton GetUserButton() => new(MainTheme.InfoBrush3, MainTheme.InfoBrush1, MainTheme.InfoBrush2) {
            Content = ". . .",
            FontFamily = MainTheme.RobotoFont,
            FontSize = 16,
            Foreground = MainTheme.NeutralBrush1,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            CornerRadius = new(5d),
            Padding = new(10),
        };

        private static TextBlock GetUserLabel(string text) => new() {
            Text = text,
            FontFamily = MainTheme.RobotoFont,
            FontSize = 16,
            Foreground = MainTheme.NeutralBrush1,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        private static Image GetLogo() {
            var uri = new Uri("avares://Stonebot/Assets/logo.png");
            var assetStream = AssetLoader.Open(uri);
            var bitmap = new Bitmap(assetStream);
            var logo = new Image {
                Source = bitmap,
                Stretch = Stretch.UniformToFill,
                Margin = new(10),
            };
            RenderOptions.SetBitmapInterpolationMode(logo, BitmapInterpolationMode.None);
            return logo;
        }
    }
}
