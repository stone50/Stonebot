namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Buttons.Links;
    using CustomControls.Popups;

    internal class ConfirmAuthPopup : SPopup {
        public static ConfirmAuthPopup Create() {
            var cancelButton = GetCancelButton();
            var okButton = GetOkButton();
            var footer = GetFooter(cancelButton, okButton);
            var cancelOkPopup = new ConfirmAuthPopup("Authorize", footer);
            cancelButton.Click += cancelOkPopup.OnCancelButtonClick;
            okButton.Click += cancelOkPopup.OnOkButtonClick;
            return cancelOkPopup;
        }

        public void Show(string userCode, string verificationUri, Action onCancel, Action onOk) {
            this.onCancel = onCancel;
            this.onOk = onOk;
            body.Inlines = [
                new Run($"Please enter this code into your browser:\n{userCode}"),
                // TODO: copy button
                new Run($"\nIf a browser tab did not automatically open, please go to:\n"),
                new UrlLink(verificationUri).GetInline(),
            ];
            IsVisible = true;
        }

        private Action? onCancel;
        private Action? onOk;

        private ConfirmAuthPopup(string title, Control footer) : base(title, null, footer) { }

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
            Content = "Done",
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
