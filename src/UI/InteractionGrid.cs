namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Command.CommandCardControls;
    using CustomControls;
    using CustomControls.Popups;
    using Scripting;

    internal class InteractionGrid : WrapPanel {
        public InteractionGrid() {
            instance = this;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public void Init(DeleteCommandPopup deleteCommandPopup) {
            this.deleteCommandPopup = deleteCommandPopup;
            var sortedCommands = CommandManager.Commands.OrderBy(x => x.Name);
            foreach (var command in sortedCommands) {
                var swappableCommandDisplay = new Swappable();
                var commandCard = new CommandCard(command, deleteCommandPopup, swappableCommandDisplay);
                var commandStub = new CommandStub(command, swappableCommandDisplay);
                swappableCommandDisplay.Init(commandStub, commandCard);
                Children.Add(swappableCommandDisplay);
            }
        }

        public static void Update() {
            var displayToggleStates = new Dictionary<Scripting.Command, bool>();
            foreach (var child in instance!.Children) {
                var commandCard = (CommandStub)((Swappable)child).Children[0];
                displayToggleStates[commandCard.Command] = !commandCard.IsVisible;
            }

            instance!.Children.Clear();
            var sortedCommands = CommandManager.Commands.OrderBy(x => x.Name);
            foreach (var command in sortedCommands) {
                var swappableCommandDisplay = new Swappable();
                var commandCard = new CommandCard(command, instance.deleteCommandPopup!, swappableCommandDisplay);
                var commandStub = new CommandStub(command, swappableCommandDisplay);
                swappableCommandDisplay.Init(commandStub, commandCard);
                instance.Children.Add(swappableCommandDisplay);
                if (displayToggleStates[command]) {
                    swappableCommandDisplay.Swap();
                }
            }
        }

        private static InteractionGrid? instance;
        private DeleteCommandPopup? deleteCommandPopup;
    }
}
