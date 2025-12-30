namespace StonebotCore.Twitch {
    using ResourceManagement;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Api.Core.Enums;

    internal static partial class Auth {
        internal static DateTime AccessTokenExpiration { get; private set; } = DateTime.UtcNow;
        private const string _redirectUri = "http://localhost:57043/bot-auth";
        private static readonly Regex _authorizationCodeRegex = AuthorizationCodeRegex();
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
            var clientId = StonebotCore.Access.Config.ClientId;
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

        internal static async Task AuthorizeAsync(
            string redirectHtml,
            CancellationToken cancellationToken
        ) {
            var api = Access.API;
            var apiAuth = api.Auth;
            var apiSettings = api.Settings;
            var clientId = StonebotCore.Access.Config.ClientId;
            apiSettings.ClientId = clientId;
            var state = GetState();
            var authorizationUrl = apiAuth.GetAuthorizationCodeUrl(
                redirectUri: _redirectUri,
                scopes: _scopes,
                forceVerify: true,
                state
            );
            string? request = null;
            var tcpListener = new TcpListener(IPAddress.Loopback, 57043);
            tcpListener.Start();
            _ = Process.Start(new ProcessStartInfo() {
                FileName = authorizationUrl,
                UseShellExecute = true,
            });
            try {
                using var client = await tcpListener.AcceptTcpClientAsync(cancellationToken);
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream) { AutoFlush = true };
                request = await reader.ReadLineAsync(cancellationToken);
                while (!string.IsNullOrEmpty(await reader.ReadLineAsync(cancellationToken))) { }

                await writer.WriteAsync(
                    "HTTP/1.1 200 OK\r\n" +
                    "Content-Type: text/html; charset=UTF-8\r\n" +
                    $"Content-Length: {redirectHtml.Length}\r\n" +
                    "Connection: close\r\n\r\n"
                );
                await writer.WriteAsync(redirectHtml);
            } catch {
                throw;
            } finally {
                tcpListener.Stop();
            }

            if (request == null) {
                throw new Exception("Could not read request");
            }

            var authorizationCodeMatch = _authorizationCodeRegex.Match(request);
            if (!authorizationCodeMatch.Success) {
                throw new Exception("Invalid response format");
            }

            var code = authorizationCodeMatch.Groups[1].Value;
            var matchedState = authorizationCodeMatch.Groups[2].Value;
            if (matchedState != state) {
                throw new Exception("States do not match");
            }

            var clientSecret = await ResourceManager.LoadTwitchClientSecretAsync(cancellationToken);
            var tokenResponse = await apiAuth.GetAccessTokenFromCodeAsync(
                code: code,
                clientSecret: clientSecret,
                redirectUri: _redirectUri
            );
            AccessTokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            await ResourceManager.SaveTwitchRefreshTokenAsync(
                refreshToken: tokenResponse.RefreshToken,
                cancellationToken
            );
            apiSettings.AccessToken = tokenResponse.AccessToken;
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

        [GeneratedRegex(@"^GET /bot-auth\?code=(\w{30})&scope=.*&state=(\w{30})", RegexOptions.Compiled)]
        private static partial Regex AuthorizationCodeRegex();
    }
}
