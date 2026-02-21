namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class HugCommand() : Command("hug", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            TwitchClient twitchClient,
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var emote = Random.Shared.Next(10) == 0 ? "pedroJAM" : "catKISS";
            return twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"{emote} {args.ChatMessage.DisplayName} {emote}");
        }
    }
}
