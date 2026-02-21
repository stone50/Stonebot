namespace StonebotDaemon.TwitchMessageHandling.Commands {
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal abstract class Command(string keyword, PermissionLevel permissionLevel) {
        internal readonly string Keyword = keyword;
        internal PermissionLevel PermissionLevel = permissionLevel;
        // TODO: add cooldown
        // TODO: add execution cancellation token

        internal abstract Task ExecuteAsync(
            TwitchClient twitchClient,
            OnChatCommandReceivedArgs args,
            CancellationToken cancellationToken
        );
    }
}
