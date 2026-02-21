namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using StonebotDaemon.Models;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class AddQuoteCommand() : Command("addquote", PermissionLevel.VIP) {
        internal override async Task ExecuteAsync(
            TwitchClient twitchClient,
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var quotes = await ResourceManager.LoadQuotesAsync(cancellationToken).ConfigureAwait(false);
            var chatMessage = args.ChatMessage;
            var quote = new Quote() {
                QuoteText = TwitchMessageHandler.GetFilteredText(args.Command.ArgumentsAsString),
                SpokenBy = chatMessage.Channel,
                DateAdded = DateTime.UtcNow,
                AddedBy = chatMessage.DisplayName
            };
            Quote[] newQuotes = [.. quotes, quote];
            await ResourceManager.SaveQuotesAsync(newQuotes, cancellationToken).ConfigureAwait(false);
            await twitchClient.SendReplyAsync(args.ChatMessage.Channel, args.ChatMessage.Id, $"quote {newQuotes.Length} added").ConfigureAwait(false);
        }
    }
}
