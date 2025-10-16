namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Command.CommandCardControls;
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
                Children.Add(new CommandCard(command, deleteCommandPopup));
            }
        }

        public static void Update() {
            instance!.Children.Clear();
            var sortedCommands = CommandManager.Commands.OrderBy(x => x.Name);
            foreach (var command in sortedCommands) {
                instance.Children.Add(new CommandCard(command, instance.deleteCommandPopup!));
            }
        }

        private static InteractionGrid? instance;
        private DeleteCommandPopup? deleteCommandPopup;
    }
}
