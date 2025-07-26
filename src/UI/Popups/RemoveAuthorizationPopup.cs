namespace Stonebot.UI.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Buttons;
    using Buttons.Links;

    internal class RemoveAuthorizationPopup : SPopup {
        public Action? OnCancel;
        public Action? OnOk;

        public static RemoveAuthorizationPopup Create() {
            var cancelButton = GetCancelButton();
            var okButton = GetOkButton();
            var footer = GetFooter(cancelButton, okButton);
            var removeAuthorizationPopup = new RemoveAuthorizationPopup(footer);
            cancelButton.Click += GetOnCancelClick(removeAuthorizationPopup);
            okButton.Click += GetOnOkClick(removeAuthorizationPopup);
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

        private static SGrid GetFooter(DangerButton cancelButton, InfoButton okButton) => new([
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

        private static DangerButton GetCancelButton() => new() {
            Content = "Cancel",
            Margin = new(0d, 0d, 10d, 0d),
            MaxHeight = 50d,
        };

        private static InfoButton GetOkButton() => new() {
            Content = "Ok",
            Margin = new(10d, 0d, 0d, 0d),
            MaxHeight = 50d,
        };

        private static EventHandler<RoutedEventArgs> GetOnCancelClick(RemoveAuthorizationPopup removeAuthorizationPopup) => (_, _) => {
            removeAuthorizationPopup.OnCancel!();
            removeAuthorizationPopup.OnCancel = null;
            removeAuthorizationPopup.IsVisible = false;
        };

        private static EventHandler<RoutedEventArgs> GetOnOkClick(RemoveAuthorizationPopup removeAuthorizationPopup) => (_, _) => {
            removeAuthorizationPopup.OnOk!();
            removeAuthorizationPopup.OnOk = null;
            removeAuthorizationPopup.IsVisible = false;
        };
    }
}
