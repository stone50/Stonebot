namespace Stonebot.Twitch {
    using Models.Data;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static partial class Auth {
        public static AccessTokenData RefreshAccessToken(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken) => PostForAccessTokenData(new() {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "grant_type", "refresh_token" },
            { "refresh_token",  refreshToken },
        }, cancellationToken);

        public static AccessTokenData GetAccessToken(string clientId, string clientSecret, string[] scopes, CancellationToken cancellationToken) => PostForAccessTokenData(new() {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "code",  GetAuthorizationCode(clientId, scopes) },
            { "grant_type", "authorization_code" },
            { "redirect_uri", $"http://localhost:{Config.AuthorizationPort}" },
        }, cancellationToken);

        private static readonly Regex codeRegex = CodeRegex();

        private static AccessTokenData PostForAccessTokenData(Dictionary<string, string> queryParams, CancellationToken cancellationToken) {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/token", queryParams);
            return Utils.SendPostRequest(Cache.DefaultClient, url, JsonContext.Default.AccessTokenData, cancellationToken);
        }

        private static string GetAuthorizationCode(string clientId, string[] scopes) {
            var server = new TcpListener(IPAddress.Loopback, Config.AuthorizationPort);
            server.Start();
            try {
                var state = GetState();
                StartAuthorizationProcess(clientId, scopes, state);
                return GetAuthorizationCodeFromServer(server, state);
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

        private static string GetAuthorizationCodeFromServer(TcpListener server, string state) {
            using var client = server.AcceptTcpClient();
            var stream = client.GetStream();
            try {
                var message = GetMessageFromNetworkStream(stream);
                if (!message.Contains($"state={state}")) {
                    throw new Exception("Response did not contain the correct state.");
                }

                var authorizationCode = codeRegex.Match(message).Groups["code"].Value;
                WriteToStream(stream, "HTTP/1.1 200 OK<html><head><title>Authorization Succeeded</title></head><body><h1>Authorization Success!</h1><p>You can close this tab.</p></body></html>");
                return authorizationCode;
            } catch (OperationCanceledException) {
                throw;
            } catch (Exception e) {
                WriteToStream(stream, $"HTTP/1.1 400 Bad Request<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>{e.Message}</p><p>See logs for more details.</p></body></html>");
                throw;
            } finally {
                stream.Dispose();
            }
        }

        private static string GetMessageFromNetworkStream(NetworkStream stream) {
            var buffer = new byte[1024];
            var numCharsRead = stream.Read(buffer);
            return Encoding.UTF8.GetString(buffer, 0, numCharsRead);
        }

        private static void WriteToStream(NetworkStream stream, string message) => stream.Write(Encoding.UTF8.GetBytes(message));

        [GeneratedRegex(@"[?&]code=(?<code>[^&\s]+)")]
        private static partial Regex CodeRegex();
    }
}
