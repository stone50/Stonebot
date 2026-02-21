namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class UnlurkCommand() : Command("unlurk", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            TwitchClient twitchClient,
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) => twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, "welcome back!");
    }
}
