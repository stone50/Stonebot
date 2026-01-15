namespace StonebotCore.Twitch.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class UnlurkCommand() : Command("unlurk", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) => ReplyAsync(args.ChatMessage, "welcome back!");
    }
}
