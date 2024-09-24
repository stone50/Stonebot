namespace StoneBot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal static class CommandHandler {
        public static Dictionary<string, Command> Commands = new() {
            { "commands", new TogglableCommand(UseActions.Commands) },
            { "enablecommand", new(UseActions.EnableCommand) { PermissionLevel = PermissionLevel.Mod } },
            { "disablecommand", new(UseActions.DisableCommand) { PermissionLevel = PermissionLevel.Mod } },
            { "enablemessage", new(UseActions.EnableMessage) { PermissionLevel = PermissionLevel.Mod } },
            { "disablemessage", new(UseActions.DisableMessage) { PermissionLevel = PermissionLevel.Mod } },
            { "enabletimer", new(UseActions.EnableTimer) { PermissionLevel = PermissionLevel.Mod } },
            { "disabletimer", new(UseActions.DisableTimer) { PermissionLevel = PermissionLevel.Mod } },
            { "addquote", new TogglableCommand(UseActions.AddQuote) { PermissionLevel = PermissionLevel.Mod } },
            { "deletequote", new TogglableCommand(UseActions.DeleteQuote) { PermissionLevel = PermissionLevel.Mod } },
            { "editquote", new TogglableCommand(UseActions.EditQuote) { PermissionLevel = PermissionLevel.Mod } },
            { "quote", new TogglableCommand(UseActions.Quote) },
            { "feed", new TogglableCommand(UseActions.Feed) },
            { "hug", new TogglableCommand(UseActions.Hug) },
            { "lurk", new TogglableCommand(UseActions.Lurk) },
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
