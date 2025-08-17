namespace Stonebot.UI.CustomControls.Popups {
    using Buttons;

    internal class CancelPopup : SPopup {
        public static CancelPopup Create(string title) {
            var cancelButton = GetCancelButton();
            return new(title, cancelButton);
        }

        public void Show(Action onCancel) {
            OnCancel = onCancel;
            IsVisible = true;
        }

        public void Hide() {
            IsVisible = false;
            OnCancel = null;
        }

        private Action? OnCancel;

        private CancelPopup(string title, DangerButton cancelButton) : base(title, null, cancelButton) => cancelButton.Click += (_, _) => {
            OnCancel!();
            OnCancel = null;
            IsVisible = false;
        };

        private static DangerButton GetCancelButton() => new() {
            Content = "Cancel",
            Margin = new(0d, 10d, 0d, 0d),
            MaxHeight = 50d,
        };
    }
}
