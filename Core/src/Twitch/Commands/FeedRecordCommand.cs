namespace StonebotCore.Twitch.Commands {
    using StonebotCore.ResourceManagement;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class FeedRecordCommand() : Command("feedrecord", PermissionLevel.Viewer) {
        internal override async Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var feedData = await ResourceManager.LoadFeedDataAsync(cancellationToken);
            await ReplyAsync(args.ChatMessage, $"The record is {feedData.RecordCount}, by {feedData.RecordHolder}");
        }
    }
}
