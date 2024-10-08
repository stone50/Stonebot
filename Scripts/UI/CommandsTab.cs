﻿namespace Stonebot.Scripts.UI {
    using Command;
    using Godot;

    internal partial class CommandsTab : Control {
        public override void _Ready() {
            foreach (var command in CommandHandler.Commands) {
                if (command is TogglableCommand togglableCommand) {
                    var commandPanel = Resources.TogglableCommandPanelScene.Instantiate<TogglableCommandPanel>();
                    commandPanel.Init(togglableCommand);
                    CommandsContainer.AddChild(commandPanel);
                } else {
                    var commandPanel = Resources.CommandPanelScene.Instantiate<CommandPanel>();
                    commandPanel.Init(command);
                    CommandsContainer.AddChild(commandPanel);
                }
            }
        }

        [Export]
        private Container CommandsContainer = null!;

    }
}
