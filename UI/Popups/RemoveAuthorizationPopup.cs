namespace Stonebot.UI.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Buttons;
    using Stonebot.UI.Buttons.Links;

    internal class RemoveAuthorizationPopup : SPopup {
        public Action? OnCancel;
        public Action? OnOk;

        public static RemoveAuthorizationPopup Create() {
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                Margin = new(0d, 0d, 10d, 0d),
                MaxHeight = 50d,
            };
            var okButton = new InfoButton() {
                Content = "Ok",
                Margin = new(10d, 0d, 0d, 0d),
                MaxHeight = 50d,
            };
            var footer = new SGrid([
                GridLength.Auto,
            ], [
                GridLength.Star,
                GridLength.Star,
            ], [
                cancelButton,
                okButton,
            ]) {
                Margin = new(0d, 10d, 0d, 0d),
            };
            var removeAuthorizationPopup = new RemoveAuthorizationPopup(footer);
            cancelButton.Click += (_, _) => {
                removeAuthorizationPopup.OnCancel!();
                removeAuthorizationPopup.OnCancel = null;
                removeAuthorizationPopup.IsVisible = false;
            };
            okButton.Click += (_, _) => {
                removeAuthorizationPopup.OnOk!();
                removeAuthorizationPopup.OnOk = null;
                removeAuthorizationPopup.IsVisible = false;
            };
            return removeAuthorizationPopup;
        }

        public void Show(Action onCancel, Action onOk) {
            OnCancel = onCancel;
            OnOk = onOk;
            IsVisible = true;
        }

        private RemoveAuthorizationPopup(Control footer) : base("Remove Cached Authorization?", [
            new Run("This will only remove cached authorization data.\nTo disconnect Stonebot from Twitch, go to:\n"),
            new UrlLink("https://www.twitch.tv/settings/connections").GetInline(),
            new Run("\nMake sure you are logged into the correct user."),
        ], footer) { }
    }
}
