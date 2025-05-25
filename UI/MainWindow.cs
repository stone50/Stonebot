namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Buttons;

    internal class MainWindow : Window {
        public readonly ConnectButton ConnectButton;
        public readonly BroadcasterButton BroadcasterButton;
        public readonly ChatterButton ChatterButton;
        public readonly AuthorizePopup AuthorizePopup;
        public readonly RemoveAuthorizationPopup RemoveAuthorizationPopup;

        public MainWindow() {
            Title = "Stonebot";
            Width = 1000d;
            Height = 800d;
            ConnectButton = new(this) {
                FontFamily = MainTheme.RobotoFont,
                FontSize = 18d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(50d),
                Margin = new(10d),
                Padding = new(15d, 5d, 15d, 8d),
                MaxHeight = 50d,
            };
            RemoveAuthorizationPopup = new(this);
            AuthorizePopup = new(this);
            BroadcasterButton = new BroadcasterButton(
                this,
                MainTheme.InfoBrush3,
                MainTheme.InfoBrush1,
                MainTheme.InfoBrush2
            ) {
                FontFamily = MainTheme.RobotoFont,
                FontSize = 16d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(5d),
                Padding = new(10d),
            };
            ChatterButton = new ChatterButton(
                this,
                MainTheme.InfoBrush3,
                MainTheme.InfoBrush1,
                MainTheme.InfoBrush2
            ) {
                FontFamily = MainTheme.RobotoFont,
                FontSize = 16d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(5d),
                Padding = new(10d),
            };
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
                    BroadcasterButton,
                    GetUserLabel("Chatter :"),
                    ChatterButton,
                },
                Margin = new(10d),
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
                    ConnectButton,
                    users,
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
            Content = new Panel {
                Children = {
                    fullGrid,
                    RemoveAuthorizationPopup,
                    AuthorizePopup,
                }
            };
        }

        public void UpdateUsers() {
            BroadcasterButton.UpdateState();
            ChatterButton.UpdateState();
        }

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
