namespace Stonebot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class CommandHandler {
        public static Command[] Commands = new Command[] {
            new TogglableCommand("commands", UseActions.Commands),
            new("enablecommand", UseActions.EnableCommand) { PermissionLevel = PermissionLevel.Mod },
            new("disablecommand", UseActions.DisableCommand) { PermissionLevel = PermissionLevel.Mod },
            new("enablemessage", UseActions.EnableMessage) { PermissionLevel = PermissionLevel.Mod },
            new("disablemessage", UseActions.DisableMessage) { PermissionLevel = PermissionLevel.Mod },
            new("enabletimer", UseActions.EnableTimer) { PermissionLevel = PermissionLevel.Mod },
            new("disabletimer", UseActions.DisableTimer) { PermissionLevel = PermissionLevel.Mod },
            new TogglableCommand("quote", UseActions.Quote),
            new TogglableCommand("addquote", UseActions.AddQuote) { PermissionLevel = PermissionLevel.Mod } ,
            new TogglableCommand("deletequote", UseActions.DeleteQuote) { PermissionLevel = PermissionLevel.Mod },
            new TogglableCommand("editquote", UseActions.EditQuote) { PermissionLevel = PermissionLevel.Mod } ,
            new TogglableCommand("feed", UseActions.Feed),
            new TogglableCommand("feedrecord", UseActions.FeedRecord),
            new TogglableCommand("hug", UseActions.Hug),
            new TogglableCommand("lurk", UseActions.Lurk),
            new TogglableCommand("discord", UseActions.Discord),
            new TogglableCommand("yt", UseActions.YouTube)
        };

        public static bool IsCommand(string message) => message.StartsWith('!');

        public static async Task<bool> Handle(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Handling message event as command.");
            var commandString = messageEvent.Message.Text.Trim();
            if (!IsCommand(commandString)) {
                return false;
            }

            var spaceIndex = commandString.IndexOf(' ');
            var keyword = commandString.Substring(1, spaceIndex == -1 ? commandString.Length - 1 : spaceIndex - 1);
            var command = GetCommand(keyword);
            return command is not null && await command.Use(messageEvent);
        }

        public static Command? GetCommand(string keyword) => Commands.FirstOrDefault(command => command.Keyword == keyword);
    }
}
