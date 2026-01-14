namespace StonebotCore {
    using Microsoft.Extensions.Logging;
    using StonebotCore.ResourceManagement;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Interface {
        public sealed class TwitchClientLog { }

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

        public static Task RefreshTwitchAuthAsync(CancellationToken cancellationToken) =>
            Twitch.Auth.RefreshAuthorizationAsync(cancellationToken);

        public static string StartTwitchAuthorization(string redirectUrl) =>
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

        public static void ConfigureTwtichClient(ILogger<TwitchClientLog>? logger) => Twitch.Bot.ConfigureClient(logger);

        public static Task ConnectTwtichAsync() => Twitch.Access.Client.ConnectAsync();

        public static Task DisconnectTwtichAsync() => Twitch.Access.Client.DisconnectAsync();
    }
}
