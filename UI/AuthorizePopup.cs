namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Buttons;

    internal class AuthorizePopup : SPopup {
        public CancellationTokenSource? CancellationTokenSource { get; private set; }

        public AuthorizePopup() {
            var cancelButton = new SButton(MainTheme.DangerBrush3, MainTheme.DangerBrush1, MainTheme.DangerBrush2) {
                Content = "Cancel",
                FontFamily = MainTheme.RobotoFont,
                FontSize = 16d,
                Foreground = MainTheme.NeutralBrush1,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                CornerRadius = new(5d),
                Padding = new(10d),
                Margin = new(0d, 10d, 0d, 0d),
                MaxHeight = 50d,
            };
            cancelButton.Click += (_, _) => {
                CancellationTokenSource!.Cancel();
                IsVisible = false;
            };
            var fullGrid = new Grid {
                RowDefinitions = [
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                ],
                Children = {
                    new Border{
                        Background = MainTheme.PrimaryBrush1,
                        Child = new TextBlock{
                            Text = "Please authorize in your browser",
                            FontFamily = MainTheme.RobotoFont,
                            FontSize = 20d,
                            Foreground = MainTheme.NeutralBrush1,
                            Padding = new(10d),
                        },
                        CornerRadius = new(10d, 0d),
                    },
                    cancelButton,
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

        public void Show(CancellationTokenSource cancellationTokenSource) {
            CancellationTokenSource = cancellationTokenSource;
            IsVisible = true;
        }
    }
}
