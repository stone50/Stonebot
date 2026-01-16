namespace StonebotCore.Twitch.Commands {
    using StonebotCore.ResourceManagement;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class FeedCommand() : Command("feed", PermissionLevel.Viewer) {
        internal override async Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var chatMessage = args.ChatMessage;
            var feedData = await ResourceManager.LoadFeedDataAsync(cancellationToken);
            var secondsSinceLastUse = (int)(DateTime.UtcNow - feedData.DateLastUsed).TotalSeconds;
            if (secondsSinceLastUse is < 0 or > 100) {
                secondsSinceLastUse = 100;
            }

            if (Random.Shared.Next(secondsSinceLastUse) == 0) {
                feedData.Count = 0;
                await ReplyAsync(chatMessage, "popCat BARF2 BARF3 you fed the cat too fast!");
            } else {
                ++feedData.Count;
                if (feedData.Count > feedData.RecordCount) {
                    feedData.RecordCount = feedData.Count;
                    feedData.RecordHolder = chatMessage.DisplayName;
                    feedData.DateRecordSet = DateTime.UtcNow;
                }

                await ReplyAsync(chatMessage, $"popCat CrayonTime The cat has been fed {feedData.Count} time{(feedData.Count == 1 ? "" : "s")} in a row");
            }

            feedData.DateLastUsed = DateTime.UtcNow;
            await ResourceManager.SaveFeedDataAsync(feedData, cancellationToken);
        }
    }
}
