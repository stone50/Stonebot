namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Scripting;

    internal class InteractionGrid : WrapPanel {
        public InteractionGrid() {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = new(10d);
        }

        public void Update() {
            foreach (var command in CommandManager.Commands) {
                Children.Add(new CommandCard(command));
            }
        }
    }
}
