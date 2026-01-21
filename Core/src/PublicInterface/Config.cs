namespace StonebotCore.PublicInterface {
    using StonebotCore.ResourceManagement;
    using System.Threading;
    using System.Threading.Tasks;

    public static partial class Interface {
        public static Task LoadConfigAsync(CancellationToken cancellationToken) =>
           ResourceManager.LoadConfigAsync(cancellationToken);

        public static Task SaveConfigAsync(CancellationToken cancellationToken) =>
            ResourceManager.SaveConfigAsync(cancellationToken);

        public static Task SetTwitchClientIdAsync(
            string clientId,
            CancellationToken cancellationToken
        ) {
            Access.Config.TwitchClientId = clientId;
            return ResourceManager.SaveConfigAsync(cancellationToken);
        }

        public static Task SetTwitchClientSecretAsync(
            string clientSecret,
            CancellationToken cancellationToken
        ) => ResourceManager.SaveTwitchClientSecretAsync(clientSecret, cancellationToken);

        public static Task SetTwitchBotUsernameAsync(
            string botUsername,
            CancellationToken cancellationToken
        ) {
            Access.Config.TwitchBotUsername = botUsername;
            return ResourceManager.SaveConfigAsync(cancellationToken);
        }

        public static Task SetTwitchBroadcasterChannelAsync(
            string broadcasterChannel,
            CancellationToken cancellationToken
        ) {
            Access.Config.TwitchBroadcasterChannel = broadcasterChannel;
            return ResourceManager.SaveConfigAsync(cancellationToken);
        }

        // TODO: add methods for getting config values
    }
}
