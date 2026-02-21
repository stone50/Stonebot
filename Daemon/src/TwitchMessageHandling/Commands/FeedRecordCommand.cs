namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class FeedRecordCommand() : Command("feedrecord", PermissionLevel.Viewer) {
        internal override async Task ExecuteAsync(
            TwitchClient twitchClient,
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var feedData = await ResourceManager.LoadFeedDataAsync(cancellationToken);
            await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"The record is {feedData.RecordCount}, by {feedData.RecordHolder}");
        }
    }
}
