namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Media;
    using CustomControls;
    using CustomControls.Buttons;
    using CustomControls.Buttons.Links;
    using CustomControls.Popups;

    internal class ConfirmAuthPopup : SPopup {
        public ConfirmAuthPopup() : base("Authorize") {
            bodyTextBlock = GetBodyTextBlock();
            Body.Children.Add(bodyTextBlock);
            var cancelButton = GetCancelButton();
            var doneButton = GetDoneButton();
            var footerButtons = GetFooterButtons(cancelButton, doneButton);
            Footer.Children.Add(footerButtons);
        }

        private readonly STextBlock bodyTextBlock;
        private Action? onDone;

        private static SGrid GetFooterButtons(DangerButton cancelButton, SuccessButton doneButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Star,
            GridLength.Star,
        ], [
            cancelButton,
            doneButton,
        ]) {
            Margin = new(0d, 10d, 0d, 0d),
        };

        private DangerButton GetCancelButton() {
            var cancelButton = new DangerButton() {
                Content = "Cancel",
                Margin = new(0d, 0d, 10d, 0d),
                MaxHeight = 50d,
            };
            cancelButton.Click += (_, _) => {
                onDone = null;
                IsVisible = false;
            };
            return cancelButton;
        }

        private SuccessButton GetDoneButton() {
            var doneButton = new SuccessButton() {
                Content = "Done",
                Margin = new(10d, 0d, 0d, 0d),
                MaxHeight = 50d,
            };
            doneButton.Click += (_, _) => {
                onDone!();
                IsVisible = false;
            };
            return doneButton;
        }

        private static STextBlock GetBodyTextBlock() => new() {
            TextWrapping = TextWrapping.Wrap,
        };

        public void Show(string userCode, string verificationUri, Action onDone) {
            this.onDone = onDone;
            bodyTextBlock.Inlines = [
                new Run($"Please enter this code into your browser:\n{userCode}"),
                // TODO: copy button
                new Run($"\nIf a browser tab did not automatically open, please go to:\n"),
                new UrlLink(verificationUri).GetInline(),
            ];
            IsVisible = true;
        }
    }
}
