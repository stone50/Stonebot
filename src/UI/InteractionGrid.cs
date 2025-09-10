namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Command.CommandCardControls;
    using CustomControls.Buttons;
    using Scripting;

    internal class InteractionGrid : WrapPanel {
        public InteractionGrid() {
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new(10d);
            VerticalAlignment = VerticalAlignment.Top;
        }

        public void Init(NewCommandPopup newCommandPopup) {
            addCommandButton = GetAddCommandButton(newCommandPopup);
            Children.Add(addCommandButton);
            foreach (var command in CommandManager.Commands) {
                Children.Add(new CommandCard(command));
            }
        }

        public void Update() {
            Children.Clear();
            Children.Add(addCommandButton!);
            foreach (var command in CommandManager.Commands) {
                Children.Add(new CommandCard(command));
            }
        }

        private SuccessButton? addCommandButton;

        private static SuccessButton GetAddCommandButton(NewCommandPopup newCommandPopup) {
            var addCommandButton = new SuccessButton() {
                Content = "New Command",
                Height = 70d,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 400d,
            };
            addCommandButton.Click += GetOnAddCommandButtonClick(newCommandPopup);
            return addCommandButton;
        }

        private static EventHandler<RoutedEventArgs> GetOnAddCommandButtonClick(NewCommandPopup newCommandPopup) => (_, _) => newCommandPopup.Show();
    }
}
