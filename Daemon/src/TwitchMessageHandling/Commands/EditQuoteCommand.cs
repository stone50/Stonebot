namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class EditQuoteCommand() : Command("editquote", PermissionLevel.VIP) {
        internal override async Task ExecuteAsync(
            TwitchClient twitchClient,
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var quotes = await ResourceManager.LoadQuotesAsync(cancellationToken).ConfigureAwait(false);
            if (quotes.Length == 0) {
                await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, "there are no quotes yet").ConfigureAwait(false);
                return;
            }

            var filteredArgumentsAsString = TwitchMessageHandler.GetFilteredText(args.Command.ArgumentsAsString);
            var firstSpaceIndex = filteredArgumentsAsString.IndexOf(' ');
            if (firstSpaceIndex == -1) {
                await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"pick a quote from 1-{quotes.Length} then provide the new quote (ex: !editquote 1 example)").ConfigureAwait(false);
                return;
            }

            var indexArgument = filteredArgumentsAsString[..firstSpaceIndex];
            if (
                !int.TryParse(indexArgument, out var spokenIndex) ||
                spokenIndex < 1 || spokenIndex > quotes.Length
            ) {
                await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"pick a quote from 1-{quotes.Length} (ex: !editquote 1 example)").ConfigureAwait(false);
                return;
            }

            var index = spokenIndex - 1;
            quotes[index].QuoteText = filteredArgumentsAsString[(firstSpaceIndex + 1)..];
            await ResourceManager.SaveQuotesAsync(quotes, cancellationToken).ConfigureAwait(false);
            await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"quote {spokenIndex} edited").ConfigureAwait(false);
        }
    }
}
