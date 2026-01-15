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
            new Hug(),
        ];

        internal static Task OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs args) {
            var chatMessage = args.ChatMessage;
            if (chatMessage.IsMe) {
                return Task.CompletedTask;
            }

            var invokedCommand = Commands.FirstOrDefault(command => command.Keyword == args.Command.Name);
            if (invokedCommand == null) {
                return Task.CompletedTask;
            }

            if (!UserCanUseCommand(chatMessage, invokedCommand)) {
                return Task.CompletedTask;
            }

            try {
                // TODO: implement default command timeout
                return invokedCommand.ExecuteAsync(args, default);
            } catch (Exception e) {
                Access.Logger?.LogError(e, "Error executing command `{CommandName}`", invokedCommand.Keyword);
                return Task.CompletedTask;
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
