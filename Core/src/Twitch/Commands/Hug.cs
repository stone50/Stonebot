namespace StonebotCore.Twitch.Commands {
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class Hug() : Command("hug", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) {
            var chatMessage = args.ChatMessage;
            var emote = Random.Shared.Next(10) == 0 ? "pedroJAM" : "catKISS";
            return ReplyAsync(chatMessage, $"{emote} {chatMessage.DisplayName} {emote}");
        }
    }
}
