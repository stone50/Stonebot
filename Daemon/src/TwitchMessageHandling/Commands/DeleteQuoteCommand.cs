namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using StonebotDaemon.Models;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class DeleteQuoteCommand() : Command("deletequote", PermissionLevel.VIP) {
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

            if (
                !int.TryParse(TwitchMessageHandler.GetFilteredText(args.Command.ArgumentsAsString), out var spokenIndex) ||
                spokenIndex < 1 || spokenIndex > quotes.Length
            ) {
                await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"pick a quote from 1-{quotes.Length} (ex: !deletequote 1)").ConfigureAwait(false);
                return;
            }

            var index = spokenIndex - 1;
            var newQuotes = new Quote[quotes.Length - 1];
            Array.Copy(quotes, 0, newQuotes, 0, index);
            Array.Copy(quotes, index + 1, newQuotes, index, quotes.Length - index - 1);
            await ResourceManager.SaveQuotesAsync(newQuotes, cancellationToken).ConfigureAwait(false);
            await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"quote {spokenIndex} deleted").ConfigureAwait(false);
        }
    }
}
