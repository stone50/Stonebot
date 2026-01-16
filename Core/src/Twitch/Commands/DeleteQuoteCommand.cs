namespace StonebotCore.Twitch.Commands {
    using StonebotCore.Models;
    using StonebotCore.ResourceManagement;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class DeleteQuoteCommand() : Command("deletequote", PermissionLevel.VIP) {
        internal override async Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var chatMessage = args.ChatMessage;
            var quotes = await ResourceManager.LoadQuotesAsync(cancellationToken).ConfigureAwait(false);
            if (quotes.Length == 0) {
                await ReplyAsync(chatMessage, "there are no quotes yet").ConfigureAwait(false);
                return;
            }

            if (
                !int.TryParse(Bot.GetFilteredText(args.Command.ArgumentsAsString), out var spokenIndex) ||
                spokenIndex < 1 || spokenIndex > quotes.Length
            ) {
                await ReplyAsync(chatMessage, $"pick a quote from 1-{quotes.Length} (ex: !deletequote 1)").ConfigureAwait(false);
                return;
            }

            var index = spokenIndex - 1;
            var newQuotes = new Quote[quotes.Length - 1];
            Array.Copy(quotes, 0, newQuotes, 0, index);
            Array.Copy(quotes, index + 1, newQuotes, index, quotes.Length - index - 1);
            await ResourceManager.SaveQuotesAsync(newQuotes, cancellationToken).ConfigureAwait(false);
            await ReplyAsync(chatMessage, $"quote {spokenIndex} deleted").ConfigureAwait(false);
        }
    }
}
