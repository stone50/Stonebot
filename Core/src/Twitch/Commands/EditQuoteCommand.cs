namespace StonebotCore.Twitch.Commands {
    using StonebotCore.ResourceManagement;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class EditQuoteCommand() : Command("editquote", PermissionLevel.VIP) {
        internal override async Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var chatMessage = args.ChatMessage;
            var quotes = await ResourceManager.GetQuotesAsync(cancellationToken).ConfigureAwait(false);
            if (quotes.Length == 0) {
                await ReplyAsync(chatMessage, "there are no quotes yet").ConfigureAwait(false);
                return;
            }

            var filteredArgumentsAsString = Bot.GetFilteredText(args.Command.ArgumentsAsString);
            var firstSpaceIndex = filteredArgumentsAsString.IndexOf(' ');
            if (firstSpaceIndex == -1) {
                await ReplyAsync(chatMessage, $"pick a quote from 1-{quotes.Length} then provide the new quote (ex: !editquote 1 example)").ConfigureAwait(false);
                return;
            }

            var indexArgument = filteredArgumentsAsString[..firstSpaceIndex];
            if (
                !int.TryParse(indexArgument, out var spokenIndex) ||
                spokenIndex < 1 || spokenIndex > quotes.Length
            ) {
                await ReplyAsync(chatMessage, $"pick a quote from 1-{quotes.Length} (ex: !editquote 1 example)").ConfigureAwait(false);
                return;
            }

            var index = spokenIndex - 1;
            quotes[index].QuoteText = filteredArgumentsAsString[(firstSpaceIndex + 1)..];
            await ResourceManager.SaveQuotesAsync(quotes, cancellationToken).ConfigureAwait(false);
            await ReplyAsync(chatMessage, $"quote {spokenIndex} edited").ConfigureAwait(false);
        }
    }
}
