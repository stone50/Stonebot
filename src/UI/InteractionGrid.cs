namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Command.CommandCardControls;
    using CustomControls.Buttons;
    using CustomControls.Popups;
    using Scripting;

    internal class InteractionGrid : WrapPanel {
        public InteractionGrid() {
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new(10d);
            VerticalAlignment = VerticalAlignment.Top;
        }

        public void Init(NewCommandPopup newCommandPopup, DeleteCommandPopup deleteCommandPopup) {
            this.deleteCommandPopup = deleteCommandPopup;
            addCommandButton = GetAddCommandButton(newCommandPopup);
            Children.Add(addCommandButton);
            var sortedCommands = CommandManager.Commands.OrderBy(x => x.Name);
            foreach (var command in sortedCommands) {
                Children.Add(new CommandCard(command, deleteCommandPopup));
            }
        }

        public void Update() {
            Children.Clear();
            Children.Add(addCommandButton!);
            var sortedCommands = CommandManager.Commands.OrderBy(x => x.Name);
            foreach (var command in sortedCommands) {
                Children.Add(new CommandCard(command, deleteCommandPopup!));
            }
        }

        private SuccessButton? addCommandButton;
        private DeleteCommandPopup? deleteCommandPopup;

        private static SuccessButton GetAddCommandButton(NewCommandPopup newCommandPopup) {
            var addCommandButton = new SuccessButton() {
                Content = "New Command",
                BorderThickness = new(100d),
                Width = 400d,
            };
            addCommandButton.Click += GetOnAddCommandButtonClick(newCommandPopup);
            return addCommandButton;
        }

        private static EventHandler<RoutedEventArgs> GetOnAddCommandButtonClick(NewCommandPopup newCommandPopup) => (_, _) => newCommandPopup.Show();
    }
}
