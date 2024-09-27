namespace StoneBot.Scripts.UI {
    using Godot;
    using StoneBot.Scripts.Command;

    internal partial class UI : Control {
        public static PackedScene CommandPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/CommandPanel.tscn");
        public static PackedScene TogglableCommandPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/TogglableCommandPanel.tscn");

        [Export]
        private Container LogsContainer = null!;
        [Export]
        private Container CommandsContainer = null!;
        [Export]
        private Container MessagesContainer = null!;
        [Export]
        private Container TimersContainer = null!;

        public override void _Ready() {
            foreach (var command in CommandHandler.Commands) {
                if (command is TogglableCommand togglableCommand) {
                    var commandPanel = TogglableCommandPanelScene.Instantiate<TogglableCommandPanel>();
                    commandPanel.Init(this, togglableCommand);
                    CommandsContainer.AddChild(commandPanel);
                } else {
                    var commandPanel = CommandPanelScene.Instantiate<CommandPanel>();
                    commandPanel.Init(this, command);
                    CommandsContainer.AddChild(commandPanel);
                }
            }
        }
    }
}
