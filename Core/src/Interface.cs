namespace StonebotCore {
    using StonebotCore.ResourceManagement;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Interface {
        public static Task LoadConfigAsync(CancellationToken cancellationToken) =>
            ResourceManager.LoadConfigAsync(cancellationToken);

        public static Task SaveConfigAsync(CancellationToken cancellationToken) =>
            ResourceManager.SaveConfigAsync(cancellationToken);

        public static Task SetTwitchClientIdAsync(
            string clientId,
            CancellationToken cancellationToken
        ) {
            Access.Config.ClientId = clientId;
            return ResourceManager.SaveConfigAsync(cancellationToken);
        }

        public static Task SetTwitchClientSecretAsync(
            string clientSecret,
            CancellationToken cancellationToken
        ) => ResourceManager.SaveTwitchClientSecretAsync(clientSecret, cancellationToken);

        public static Task RefreshTwitchAuthAsync(CancellationToken cancellationToken) =>
            Twitch.Auth.RefreshAuthorizationAsync(cancellationToken);

        public static Task AuthorizeTwitchAsync(
            string redirectHtml,
            CancellationToken cancellationToken
        ) => Twitch.Auth.AuthorizeAsync(redirectHtml, cancellationToken);
    }
}
