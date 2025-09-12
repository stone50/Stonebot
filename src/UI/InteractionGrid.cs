namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Command.CommandCardControls;
    using CustomControls.Popups;
    using Scripting;

    internal class InteractionGrid : WrapPanel {
        public InteractionGrid() {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public void Init(DeleteCommandPopup deleteCommandPopup) {
            this.deleteCommandPopup = deleteCommandPopup;
            var sortedCommands = CommandManager.Commands.OrderBy(x => x.Name);
            foreach (var command in sortedCommands) {
                Children.Add(new CommandCard(command, deleteCommandPopup, this));
            }
        }

        public void Update() {
            Children.Clear();
            var sortedCommands = CommandManager.Commands.OrderBy(x => x.Name);
            foreach (var command in sortedCommands) {
                Children.Add(new CommandCard(command, deleteCommandPopup!, this));
            }
        }

        private DeleteCommandPopup? deleteCommandPopup;
    }
}
