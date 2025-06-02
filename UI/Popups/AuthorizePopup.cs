namespace Stonebot.UI.Popups {
    using Avalonia.Controls;
    using Buttons;

    internal class AuthorizePopup : SPopup {
        public CancellationTokenSource? CancellationTokenSource;

        public static AuthorizePopup Create() {
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                Margin = new(0d, 10d, 0d, 0d),
                MaxHeight = 50d,
            };
            var authorizePopup = new AuthorizePopup(cancelButton);
            cancelButton.Click += (_, _) => {
                authorizePopup.CancellationTokenSource!.Cancel();
                authorizePopup.CancellationTokenSource = null;
                authorizePopup.IsVisible = false;
            };
            return authorizePopup;
        }

        public void Show(CancellationTokenSource cancellationTokenSource) {
            CancellationTokenSource = cancellationTokenSource;
            IsVisible = true;
        }

        private AuthorizePopup(Control footer) : base("Please Authorize in Your Browser", null, footer) { }
    }
}
