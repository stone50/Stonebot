namespace StonebotCore.Twitch.Commands {
    using StonebotCore.ResourceManagement;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class QuoteCommand() : Command("quote", PermissionLevel.Viewer) {
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
            int index;
            if (string.IsNullOrWhiteSpace(command.ArgumentsAsString)) {
                index = Random.Shared.Next(0, quotes.Length);
            } else {
                if (
                    !int.TryParse(args.Command.ArgumentsAsString.Trim(), out var spokenIndex) ||
                    spokenIndex < 1 || spokenIndex > quotes.Length
                ) {
                    await ReplyAsync(chatMessage, $"pick a quote from 1-{quotes.Length} (ex: !quote 1)").ConfigureAwait(false);
                    return;
                }

                index = spokenIndex - 1;
            }

            var quote = quotes[index];
            await ReplyAsync(chatMessage, $"[{index + 1}] \"{quote.QuoteText}\" - {quote.SpokenBy}").ConfigureAwait(false);
        }
    }
}
