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

        public static string StartAuthorization(string redirectUrl) =>
            Twitch.Auth.StartAuthorization(redirectUrl);

        public static Task AuthorizeTwitchFromCodeAsync(
            string authorizationCode,
            string redirectUrl,
            CancellationToken cancellationToken
        ) => Twitch.Auth.AuthorizeFromCodeAsync(
            authorizationCode,
            redirectUrl,
            cancellationToken
        );
    }
}
