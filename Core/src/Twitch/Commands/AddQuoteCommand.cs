namespace StonebotCore.Twitch.Commands {
    using StonebotCore.Models;
    using StonebotCore.ResourceManagement;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class AddQuoteCommand() : Command("addquote", PermissionLevel.VIP) {
        internal override async Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var quotes = await ResourceManager.GetQuotesAsync(cancellationToken).ConfigureAwait(false);
            var chatMessage = args.ChatMessage;
            var quote = new Quote() {
                QuoteText = Bot.GetVisibleCharacters(args.Command.ArgumentsAsString),
                SpokenBy = chatMessage.Channel,
                DateAdded = DateTime.UtcNow,
                AddedBy = chatMessage.DisplayName
            };
            Quote[] newQuotes = [.. quotes, quote];
            await ResourceManager.SaveQuotesAsync(newQuotes, cancellationToken).ConfigureAwait(false);
            await ReplyAsync(chatMessage, $"quote {newQuotes.Length} added").ConfigureAwait(false);
        }
    }
}
