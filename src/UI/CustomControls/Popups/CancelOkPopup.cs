namespace Stonebot.UI.CustomControls.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Interactivity;
    using Buttons;

    internal class CancelOkPopup : SPopup {
        public static CancelOkPopup Create(string title, InlineCollection? bodyInlines) {
            var cancelButton = GetCancelButton();
            var okButton = GetOkButton();
            var footer = GetFooter(cancelButton, okButton);
            var cancelOkPopup = new CancelOkPopup(title, bodyInlines, footer);
            cancelButton.Click += cancelOkPopup.OnCancelButtonClick;
            okButton.Click += cancelOkPopup.OnOkButtonClick;
            return cancelOkPopup;
        }

        public void Show(Action onCancel, Action onOk) {
            this.onCancel = onCancel;
            this.onOk = onOk;
            IsVisible = true;
        }

        private Action? onCancel;
        private Action? onOk;

        private CancelOkPopup(string title, InlineCollection? bodyInlines, Control footer) : base(title, bodyInlines, footer) { }

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
