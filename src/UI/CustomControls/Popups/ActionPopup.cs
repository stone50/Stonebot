namespace Stonebot.UI.CustomControls.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Buttons;

    internal class ActionPopup : SPopup {
        public ActionPopup(Action onOk) : base() => Init(onOk);

        public ActionPopup(string title, Action onOk) : base(title) => Init(onOk);

        public ActionPopup(string title, InlineCollection inlines, Action onOk) : base(title, inlines) => Init(onOk);

        private void Init(Action onOk) {
            var cancelButton = GetCancelButton();
            var okButton = GetOkButton(onOk);
            var footerButtons = GetFooterButtons(cancelButton, okButton);
            Footer.Children.Add(footerButtons);
        }

        private static SGrid GetFooterButtons(DangerButton cancelButton, SuccessButton okButton) => new([
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

        private DangerButton GetCancelButton() {
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                Margin = new(0d, 0d, 10d, 0d),
                MaxHeight = 50d,
            };
            cancelButton.Click += (_, _) => IsVisible = false;
            return cancelButton;
        }

        private SuccessButton GetOkButton(Action onOk) {
            var okButton = new SuccessButton() {
                Content = "Ok",
                Margin = new(10d, 0d, 0d, 0d),
                MaxHeight = 50d,
            };
            okButton.Click += (_, _) => {
                onOk();
                IsVisible = false;
            };
            return okButton;
        }
    }
}
