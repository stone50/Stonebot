namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class QuoteCommand() : Command("quote", PermissionLevel.Viewer) {
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
            int index;
            if (string.IsNullOrWhiteSpace(filteredArgumentsAsString)) {
                index = Random.Shared.Next(0, quotes.Length);
            } else {
                if (
                    !int.TryParse(filteredArgumentsAsString, out var spokenIndex) ||
                    spokenIndex < 1 || spokenIndex > quotes.Length
                ) {
                    await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"pick a quote from 1-{quotes.Length} (ex: !quote 1)").ConfigureAwait(false);
                    return;
                }

                index = spokenIndex - 1;
            }

            var quote = quotes[index];
            await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"[{index + 1}] \"{quote.QuoteText}\" - {quote.SpokenBy}").ConfigureAwait(false);
        }
    }
}
