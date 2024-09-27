namespace StoneBot.Scripts.UI {
    using Command;
    using Godot;

    internal partial class CommandsTab : Control {
        public static PackedScene CommandPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/CommandPanel.tscn");
        public static PackedScene TogglableCommandPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/TogglableCommandPanel.tscn");

        [Export]
        private Container CommandsContainer = null!;

        public override void _Ready() {
            foreach (var command in CommandHandler.Commands) {
                if (command is TogglableCommand togglableCommand) {
                    var commandPanel = TogglableCommandPanelScene.Instantiate<TogglableCommandPanel>();
                    commandPanel.Init(togglableCommand);
                    CommandsContainer.AddChild(commandPanel);
                } else {
                    var commandPanel = CommandPanelScene.Instantiate<CommandPanel>();
                    commandPanel.Init(command);
                    CommandsContainer.AddChild(commandPanel);
                }
            }
        }
    }
}
