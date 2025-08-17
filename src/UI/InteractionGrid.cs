namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using CommandCardControls;
    using Scripting;

    internal class InteractionGrid : WrapPanel {
        public InteractionGrid() {
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new(10d);
            VerticalAlignment = VerticalAlignment.Top;
        }

        public void Init() {
            foreach (var command in CommandManager.Commands) {
                Children.Add(new CommandCard(command));
            }
        }
    }
}
