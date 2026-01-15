namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using StonebotCore.Twitch.Commands;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;

    internal static class CommandManager {
        internal static readonly Command[] Commands = [
            new AddQuoteCommand(),
            new DeleteQuoteCommand(),
            new DiscordCommand(),
            new EditQuoteCommand(),
            new HugCommand(),
            new LurkCommand(),
            new QuoteCommand(),
            new UnlurkCommand(),
            new YoutubeCommand()
        ];

        internal static async Task OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs args) {
            var chatMessage = args.ChatMessage;
            if (chatMessage.IsMe) {
                return;
            }

            var invokedCommand = Commands.FirstOrDefault(command => command.Keyword == args.Command.Name);
            if (invokedCommand == null) {
                return;
            }

            if (!UserCanUseCommand(chatMessage, invokedCommand)) {
                return;
            }

            try {
                // TODO: implement default command timeout
                if (Access.Logger?.IsEnabled(LogLevel.Information) ?? false) {
                    Access.Logger?.LogInformation("Using command `{CommandName}`", invokedCommand.Keyword);
                }

                await invokedCommand.ExecuteAsync(args, default).ConfigureAwait(false);
            } catch (Exception e) {
                Access.Logger?.LogError(e, "Error executing command `{CommandName}`", invokedCommand.Keyword);
                return;
            }
        }

        private static bool UserCanUseCommand(ChatMessage chatMessage, Command command) {
            var userLevel = chatMessage.IsBroadcaster
                ? (int)PermissionLevel.Broadcaster
                : chatMessage.UserDetail.IsModerator
                ? (int)PermissionLevel.Mod
                : chatMessage.UserDetail.IsVip
                ? (int)PermissionLevel.VIP
                : chatMessage.UserDetail.IsSubscriber
                ? (int)PermissionLevel.Sub
                : (int)PermissionLevel.Viewer;

            return userLevel >= (int)command.PermissionLevel;
        }
    }
}
