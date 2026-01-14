namespace StonebotCore.Twitch {
    using StonebotCore.Twitch.Commands;
    using System.Linq;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal static class CommandManager {
        internal static readonly Command[] Commands = [
            new Hug()
        ];

        internal static async Task OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs args) =>
            // TODO: implement default command timeout
            Commands.FirstOrDefault(command => command.Keyword == args.Command.Name)?.ExecuteAsync(
                args,
                Access.Logger!,
                default
            );
    }
}
