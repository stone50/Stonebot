namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Layout;
    using Avalonia.Media;
    using Buttons;

    internal class RemoveAuthorizationPopup : SPopup {
        public Action? OnCancel;
        public Action? OnOk;

        public RemoveAuthorizationPopup(MainPanel mainPanel) : base(mainPanel) {
            var cancelButton = new SButton(
                MainTheme.DangerBrush3,
                MainTheme.DangerBrush1,
                MainTheme.DangerBrush2
            ) {
                Content = "Cancel",
                FontFamily = MainTheme.Font,
                FontSize = 18d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(5d),
                Padding = new(10d),
                Margin = new(0d, 0d, 10d, 0d),
                MaxHeight = 50d,
            };
            cancelButton.Click += (_, _) => {
                OnCancel!();
                OnCancel = null;
                IsVisible = false;
            };
            var okButton = new SButton(
                MainTheme.InfoBrush3,
                MainTheme.InfoBrush1,
                MainTheme.InfoBrush2
            ) {
                Content = "Ok",
                FontFamily = MainTheme.Font,
                FontSize = 18d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(5d),
                Padding = new(10d),
                Margin = new(10d, 0d, 0d, 0d),
                MaxHeight = 50d,
            };
            okButton.Click += (_, _) => {
                OnOk!();
                OnOk = null;
                IsVisible = false;
            };
            var footer = new Grid {
                ColumnDefinitions = [
                    new(GridLength.Star),
                    new(GridLength.Star),
                ],
                Children = {
                    cancelButton,
                    okButton,
                },
                Margin = new(0d, 10d, 0d, 0d),
            };
            for (var i = 0; i < footer.Children.Count; ++i) {
                Grid.SetColumn(footer.Children[i], i);
            }

            var twitchConnectionsLink = new Link {
                Content = "https://www.twitch.tv/settings/connections",
                FontFamily = MainTheme.Font,
                FontSize = 18d,
            };
            var fullGrid = new Grid {
                RowDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                Children = {
                    new Border{
                        Background = MainTheme.PrimaryBrush1,
                        Child = new TextBlock{
                            Text = "Remove Cached Authorization?",
                            FontFamily = MainTheme.Font,
                            FontSize = 24d,
                            Foreground = MainTheme.NeutralBrush1,
                            Padding = new(10d),
                        },
                        CornerRadius = new(10d, 0d),
                    },
                    new TextBlock {
                        TextWrapping = TextWrapping.Wrap,
                        Inlines = [
                            new Run("This will only remove cached authorization data.\nTo disconnect Stonebot from Twitch, go to:\n"),
                            twitchConnectionsLink.GetInline(),
                            new Run("\nMake sure you are logged into the correct user."),
                        ],
                        FontFamily = MainTheme.Font,
                        FontSize = 18d,
                        Foreground = MainTheme.NeutralBrush1,
                        Padding = new(10d),
                    },
                    footer,
                }
            };
            for (var i = 0; i < fullGrid.Children.Count; ++i) {
                Grid.SetRow(fullGrid.Children[i], i);
            }

            Children.Add(new Border() {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MainTheme.PrimaryBrush2,
                CornerRadius = new(10d),
                Child = fullGrid,
                MaxWidth = 700d,
            });
        }

        public void Show(Action onCancel, Action onOk) {
            OnCancel = onCancel;
            OnOk = onOk;
            IsVisible = true;
        }
    }
}
