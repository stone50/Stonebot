namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using StonebotCore.Twitch.Commands;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal static class CommandManager {
        internal static readonly Command[] Commands = [
            new Hug(),
        ];

        internal static Task OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs args) {
            var invokedCommand = Commands.FirstOrDefault(command => command.Keyword == args.Command.Name);
            if (invokedCommand == null) {
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
    }
}
