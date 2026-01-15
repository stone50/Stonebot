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

            var command = args.Command;
            var indexArgument = command.ArgumentsAsList[0];
            if (
                !int.TryParse(indexArgument.Trim(), out var spokenIndex) ||
                spokenIndex < 1 || spokenIndex > quotes.Length
            ) {
                await ReplyAsync(chatMessage, $"pick a quote from 1-{quotes.Length} (ex: !editquote 1 example)").ConfigureAwait(false);
                return;
            }

            var index = spokenIndex - 1;
            var argumentsString = command.ArgumentsAsString;
            var firstSpaceIndex = argumentsString.IndexOf(' ');
            var newQuoteText = firstSpaceIndex == -1
                ? ""
                : Bot.GetVisibleCharacters(argumentsString[(firstSpaceIndex + 1)..]);
            quotes[index].QuoteText = newQuoteText;
            await ResourceManager.SaveQuotesAsync(quotes, cancellationToken).ConfigureAwait(false);
            await ReplyAsync(chatMessage, $"quote {spokenIndex} edited").ConfigureAwait(false);
        }
    }
}
