namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;

    internal abstract class Command(string keyword) {
        internal readonly string Keyword = keyword;
        // TODO: add cooldown
        // TODO: add permission level

        internal abstract Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            ILogger<Interface.TwitchClientLog> logger,
            CancellationToken cancellationToken
        );

        protected static Task ReplyAsync(
            ChatMessage chatMessage,
            string replyMessage
        ) => Access.Client.SendReplyAsync(
            chatMessage.Channel,
            chatMessage.Id,
            replyMessage
        );
    }
}
