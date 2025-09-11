namespace Stonebot.UI.CustomControls.Popups {
    using Avalonia.Controls;
    using Avalonia.Controls.Documents;
    using Avalonia.Media;
    using Buttons;
    using Scripting;

    internal class DeleteCommandPopup : SPopup {
        public DeleteCommandPopup(InteractionGrid interactionGrid) : base("Delete Command?") {
            bodyTextBlock = GetBodyTextBlock();
            Body.Children.Add(bodyTextBlock);
            var cancelButton = GetCancelButton();
            var deleteButton = GetDeleteButton(interactionGrid);
            var footerButtons = GetFooterButtons(cancelButton, deleteButton);
            Footer.Children.Add(footerButtons);
        }

        public void Show(Command commandToDelete) {
            this.commandToDelete = commandToDelete;
            bodyTextBlock.Inlines = [new Run($"Are you sure you want to delete \"!{commandToDelete.Name}\"? This will delete the script file.")];
            IsVisible = true;
        }

        private Command? commandToDelete;
        private readonly STextBlock bodyTextBlock;

        private static STextBlock GetBodyTextBlock() => new() {
            TextWrapping = TextWrapping.Wrap,
        };

        private static SGrid GetFooterButtons(InfoButton cancelButton, DangerButton deleteButton) => new([
            GridLength.Auto,
        ], [
            GridLength.Star,
            GridLength.Star,
        ], [
            cancelButton,
            deleteButton,
        ]) {
            Margin = new(0d, 10d, 0d, 0d),
        };

        private InfoButton GetCancelButton() {
            var cancelButton = new InfoButton() {
                Content = "Cancel",
                Margin = new(0d, 0d, 10d, 0d),
                MaxHeight = 50d,
            };
            cancelButton.Click += (_, _) => {
                commandToDelete = null;
                IsVisible = false;
            };
            return cancelButton;
        }

        private DangerButton GetDeleteButton(InteractionGrid interactionGrid) {
            var deleteButton = new DangerButton() {
                Content = "Delete",
                Margin = new(10d, 0d, 0d, 0d),
                MaxHeight = 50d,
            };
            deleteButton.Click += (_, _) => {
                try {
                    File.Delete(commandToDelete!.GetScriptFilePath());
                } catch (Exception e) {
                    Logger.Error(e);
                }

                _ = CommandManager.Commands.Remove(commandToDelete!);
                try {
                    CommandManager.Save();
                } catch (Exception e) {
                    Logger.Error(e);
                }

                interactionGrid.Update();
                commandToDelete = null;
                IsVisible = false;
            };
            return deleteButton;
        }
    }
}
