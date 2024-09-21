namespace StoneBot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal static class CommandHandler {
        public static Dictionary<string, Command> Commands = new() {
            { "commands", new(UseActions.Commands) },
            { "enablecommand", new(UseActions.EnableCommand) },
            { "disablecommand", new(UseActions.DisableCommand) },
            { "hug", new(UseActions.Hug) },
            { "lurk", new(UseActions.Lurk) },
        };

        public static bool IsCommand(string message) => message.StartsWith('!');

        public static async Task<bool> Handle(ChannelChatMessageEvent messageEvent) {
            var commandString = messageEvent.Message.Text.Trim();
            if (!IsCommand(commandString)) {
                return false;
            }

            var spaceIndex = commandString.IndexOf(' ');
            var keyword = commandString.Substring(1, spaceIndex == -1 ? commandString.Length - 1 : spaceIndex - 1);
            return Commands.TryGetValue(keyword, out var command) && await command.Use(messageEvent);
        }
    }
}
