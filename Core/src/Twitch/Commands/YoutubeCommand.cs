namespace StonebotCore.Twitch.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class YoutubeCommand() : Command("youtube", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) => ReplyAsync(args.ChatMessage, "https://www.youtube.com/@stone50");
    }
}
