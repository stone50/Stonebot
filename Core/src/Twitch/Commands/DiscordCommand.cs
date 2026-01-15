namespace StonebotCore.Twitch.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class DiscordCommand() : Command("discord", PermissionLevel.Viewer) {
        internal override Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        ) => ReplyAsync(args.ChatMessage, "https://discord.gg/uZQYWCFeK5");
    }
}
