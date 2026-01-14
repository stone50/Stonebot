namespace StonebotCore.Twitch {
    using ResourceManagement;
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Api.Core.Enums;

    internal static partial class Auth {
        internal static DateTime AccessTokenExpiration { get; private set; } = DateTime.UtcNow;
        private static readonly AuthScopes[] _scopes = [
            AuthScopes.User_Bot,
            AuthScopes.Channel_Bot,
            AuthScopes.Chat_Read,
            AuthScopes.Chat_Edit,
        ];

        internal static async Task RefreshAuthorizationAsync(
            CancellationToken cancellationToken
        ) {
            var api = Access.API;
            var apiSettings = api.Settings;
            var refreshToken = await ResourceManager.LoadTwitchRefreshTokenAsync(cancellationToken);
            var clientId = StonebotCore.Access.Config.TwitchClientId;
            var clientSecret = await ResourceManager.LoadTwitchClientSecretAsync(cancellationToken);
            apiSettings.ClientId = clientId;
            var refreshResponse = await api.Auth.RefreshAuthTokenAsync(
                refreshToken: refreshToken,
                clientSecret: clientSecret,
                clientId: clientId
            );
            AccessTokenExpiration = DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn);
            await ResourceManager.SaveTwitchRefreshTokenAsync(
                refreshToken: refreshResponse.RefreshToken,
                cancellationToken
            );
            apiSettings.AccessToken = refreshResponse.AccessToken;
        }

        internal static string StartAuthorization(string redirectUrl) {
            var api = Access.API;
            api.Settings.ClientId = StonebotCore.Access.Config.TwitchClientId;
            var state = GetState();
            var authorizationUrl = api.Auth.GetAuthorizationCodeUrl(
                redirectUri: redirectUrl,
                scopes: _scopes,
                forceVerify: true,
                state
            );
            _ = Process.Start(new ProcessStartInfo() {
                FileName = authorizationUrl,
                UseShellExecute = true,
            });
            return state;
        }

        internal static async Task AuthorizeFromCodeAsync(string authorizationCode, string redirectUrl, CancellationToken cancellationToken) {
            var api = Access.API;
            var clientSecret = await ResourceManager.LoadTwitchClientSecretAsync(cancellationToken);
            var tokenResponse = await api.Auth.GetAccessTokenFromCodeAsync(
                code: authorizationCode,
                clientSecret: clientSecret,
                redirectUri: redirectUrl
            );
            AccessTokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            await ResourceManager.SaveTwitchRefreshTokenAsync(
                refreshToken: tokenResponse.RefreshToken,
                cancellationToken
            );
            api.Settings.AccessToken = tokenResponse.AccessToken;
        }

        private static string GetState() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new StringBuilder(30);
            for (var i = 0; i < 30; ++i) {
                _ = result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
