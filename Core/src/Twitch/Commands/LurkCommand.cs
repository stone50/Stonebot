namespace StonebotCore.Twitch.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class LurkCommand() : Command("lurk", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) => ReplyAsync(args.ChatMessage, "thank you for your presence!");
    }
}
