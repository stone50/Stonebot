namespace StonebotCore.PublicInterface {
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public static partial class Interface {
        public sealed class TwitchClientLog { }

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

        public static void ConfigureTwitchClient(ILogger<TwitchClientLog>? logger) => Twitch.Bot.ConfigureClient(logger);

        public static Task ConnectTwtichAsync() => Twitch.Access.Client.ConnectAsync();

        public static Task DisconnectTwtichAsync() => Twitch.Access.Client.DisconnectAsync();

        // TODO: add connection status check
    }
}
