namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class LurkCommand() : Command("lurk", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            TwitchClient twitchClient,
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) => twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, "thank you for your presence!");
    }
}
