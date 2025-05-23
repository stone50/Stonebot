namespace Stonebot.Twitch {
    using Models.Data;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal static partial class Auth {
        public static Task<AccessTokenData> RefreshAccessTokenAsync(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken) => PostForAccessTokenDataAsync(new() {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token",  refreshToken },
        }, cancellationToken);

        public static async Task<AccessTokenData> GetAccessTokenAsync(string clientId, string clientSecret, string[] scopes, CancellationToken cancellationToken) => await PostForAccessTokenDataAsync(new() {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "code", await GetAuthorizationCodeAsync(clientId, scopes, cancellationToken).ConfigureAwait(false) },
            { "grant_type", "authorization_code" },
            { "redirect_uri", $"http://localhost:{Config.AuthorizationPort}" },
        }, cancellationToken).ConfigureAwait(false);

        private static readonly Regex codeRegex = CodeRegex();

        private static Task<AccessTokenData> PostForAccessTokenDataAsync(Dictionary<string, string> queryParams, CancellationToken cancellationToken) {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/token", queryParams);
            return Utils.SendPostRequestAsync(Cache.DefaultClient, url, JsonContext.Default.AccessTokenData, cancellationToken);
        }

        private static async Task<string> GetAuthorizationCodeAsync(string clientId, string[] scopes, CancellationToken cancellationToken) {
            var server = new TcpListener(IPAddress.Loopback, Config.AuthorizationPort);
            server.Start();
            try {
                var state = GetState();
                StartAuthorizationProcess(clientId, scopes, state);
                return await GetAuthorizationCodeFromServerAsync(server, state, cancellationToken);
            } finally {
                server.Stop();
            }
        }

        private static string GetState() {
            var stateChars = new char[32];
            for (var i = 0; i < stateChars.Length; ++i) {
                stateChars[i] = Constants.AllowedStateChars[RandomNumberGenerator.GetInt32(Constants.AllowedStateChars.Length)];
            }

            return new(stateChars);
        }

        private static void StartAuthorizationProcess(string clientId, string[] scopes, string state) {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/authorize", new() {
                { "client_id", clientId },
                { "force_verify",  "true"},
                { "redirect_uri", $"http://localhost:{Config.AuthorizationPort}"},
                { "response_type", "code"},
                { "scope", string.Join(" ", scopes)},
                { "state", state},
            });
            var process = new Process { StartInfo = { UseShellExecute = true, FileName = url } };
            if (!process.Start()) {
                throw new Exception("Authorization process could not start.");
            }
        }

        private static async Task<string> GetAuthorizationCodeFromServerAsync(TcpListener server, string state, CancellationToken cancellationToken) {
            using var client = await server.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
            var stream = client.GetStream();
            try {
                var message = await GetMessageFromNetworkStreamAsync(stream, cancellationToken).ConfigureAwait(false);
                if (!message.Contains($"state={state}")) {
                    throw new Exception("Response did not contain the correct state.");
                }

                var authorizationCode = codeRegex.Match(message).Groups["code"].Value;
                await SendMessageToNetworkStreamAsync(stream, "HTTP/1.1 200 OK<html><head><title>Authorization Succeeded</title></head><body><h1>Authorization Success!</h1><p>You can close this tab.</p></body></html>", cancellationToken).ConfigureAwait(false);
                return authorizationCode;
            } catch (OperationCanceledException) {
                throw;
            } catch (Exception e) {
                await SendMessageToNetworkStreamAsync(stream, $"HTTP/1.1 400 Bad Request<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>{e.Message}</p><p>See logs for more details.</p></body></html>", cancellationToken).ConfigureAwait(false);
                throw;
            } finally {
                await stream.DisposeAsync().ConfigureAwait(false);
            }
        }

        private static async Task<string> GetMessageFromNetworkStreamAsync(NetworkStream stream, CancellationToken cancellationToken) {
            var buffer = new byte[1024];
            var numCharsRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            return Encoding.UTF8.GetString(buffer, 0, numCharsRead);
        }

        private static ValueTask SendMessageToNetworkStreamAsync(NetworkStream stream, string message, CancellationToken cancellationToken) => stream.WriteAsync(Encoding.UTF8.GetBytes(message), cancellationToken);

        [GeneratedRegex(@"[?&]code=(?<code>[^&\s]+)")]
        private static partial Regex CodeRegex();
    }
}
