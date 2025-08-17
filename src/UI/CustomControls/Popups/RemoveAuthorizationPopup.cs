namespace Stonebot.UI.CustomControls.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Buttons;
    using Buttons.Links;

    internal class RemoveAuthorizationPopup : SPopup {
        public static RemoveAuthorizationPopup Create() {
            var cancelButton = GetCancelButton();
            var okButton = GetOkButton();
            var footer = GetFooter(cancelButton, okButton);
            var removeAuthorizationPopup = new RemoveAuthorizationPopup(footer);
            cancelButton.Click += removeAuthorizationPopup.OnCancelButtonClick;
            okButton.Click += removeAuthorizationPopup.OnOkButtonClick;
            return removeAuthorizationPopup;
        }

        public void Show(Action onCancel, Action onOk) {
            this.onCancel = onCancel;
            this.onOk = onOk;
            IsVisible = true;
        }

        private Action? onCancel;
        private Action? onOk;

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

        private void OnCancelButtonClick(object? sender, RoutedEventArgs args) {
            onCancel!();
            onCancel = null;
            onOk = null;
            IsVisible = false;
        }

        private void OnOkButtonClick(object? sender, RoutedEventArgs args) {
            onOk!();
            onCancel = null;
            onOk = null;
            IsVisible = false;
        }
    }
}
