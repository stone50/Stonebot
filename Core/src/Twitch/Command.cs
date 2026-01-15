namespace StonebotCore.Twitch {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;

    internal abstract class Command(string keyword, PermissionLevel permissionLevel) {
        internal readonly string Keyword = keyword;
        internal PermissionLevel PermissionLevel = permissionLevel;
        // TODO: add cooldown

        internal abstract Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
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
