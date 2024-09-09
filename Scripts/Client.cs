namespace StoneBot.Scripts {
    using Godot;
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal class Client {
        private readonly HttpClient TwitchHttpClient = new();

        private TcpServer? TwitchEventSubServer;

        public Client() => _ = InitTwitchEventSubServer();

        private bool InitTwitchEventSubServer() {
            IPAddress localhost;
            try {
                localhost = IPAddress.Parse("127.0.0.1");
            } catch (Exception e) {
                GD.PushWarning($"Could not parse IP address: {e}.");
                return false;
            }

            try {
                TwitchEventSubServer = new TcpServer(localhost, 443);
            } catch (Exception e) {
                GD.PushWarning($"Could not create server: {e}.");
                return false;
            }

            bool didTwitchEventSubServerStart;
            try {
                didTwitchEventSubServerStart = TwitchEventSubServer.Start();
            } catch (Exception e) {
                GD.PushWarning($"Could not start server: {e}.");
                return false;
            }

            if (!didTwitchEventSubServerStart) {
                GD.PushWarning($"Cannot init twitch event sub server because TwitchEventSubServer.Start failed.");
                return false;
            }

            return true;
        }

        public static async Task<string?> GetAuthorizationCode() {
            if (Configuration.AuthorizationServerPort is null) {
                GD.PushWarning("Cannot get authorization code because AuthorizationServerPort is null.");
                return null;
            }

            if (Configuration.TwitchStoneBotClientId is null) {
                GD.PushWarning("Cannot get authorization code because TwitchStoneBotClientId is null.");
                return null;
            }

            IPAddress localhost;
            try {
                localhost = IPAddress.Parse("127.0.0.1");
            } catch (Exception e) {
                GD.PushWarning($"Could not parse IP address: {e}.");
                return null;
            }

            TcpServer authorizationServer;
            try {
                authorizationServer = new TcpServer(localhost, (int)Configuration.AuthorizationServerPort);
            } catch (Exception e) {
                GD.PushWarning($"Could not create server: {e}.");
                return null;
            }

            bool didAuthorizationServerStart;
            try {
                didAuthorizationServerStart = authorizationServer.Start();
            } catch (Exception e) {
                GD.PushWarning($"Could not start server: {e}.");
                return null;
            }

            if (!didAuthorizationServerStart) {
                GD.PushWarning($"Cannot get authorization code because server start failed.");
                return null;
            }

            var promise = new TaskCompletionSource<string?>();

            var state = ""; // TODO

            authorizationServer.RequestHandler += (sender, args) => {
                var success = HandleAuthorizationServerRequest(args, promise, state);
                if (!success) {
                    GD.PushWarning("Cannot get authorization code because HandleAuthorizationServerRequest failed.");
                    _ = authorizationServer.Stop();
                    return;
                }
            };

            // TODO: update scope
            var authorizationProcess = TwitchAPI.Authorize(Configuration.TwitchStoneBotClientId, $"http://localhost:{Configuration.AuthorizationServerPort}", new[] { "channel:bot" }, state: state);

            if (authorizationProcess is null) {
                GD.PushWarning("Cannot get authorization code because authorization process failed.");
                _ = authorizationServer.Stop();
                return null;
            }

            // TODO: check if the process has quit without a code being parsed
            var result = await promise.Task;

            var success = authorizationServer.Stop();
            if (!success) {
                GD.PushWarning("Cannot get authorization code because server stop failed.");
                return null;
            }

            return await promise.Task;
        }

        private static bool HandleAuthorizationServerRequest(TcpServer.HttpRequestHandlerArgs args, TaskCompletionSource<string?> promise, string state) {
            // TODO: verify state

            var parsedCode = ParseAuthorizationCode(args.Request.URI);
            if (parsedCode is null) {
                GD.PushWarning("Cannot get authorization code because authorization code could not be parsed from request URI.");
                args.Response = new() {
                    StatusCode = 400,
                    ReasonPhrase = "Bad Request",
                    Message = "<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>Cannot get authorization code because authorization code could not be parsed from request URI.</p></body></html>"
                };
            } else {
                args.Response.Message = "<html><head><title>Authorization Succeeded</title></head><body><h1>Authorization Success! :)</h1><p>You can close this tab.</p></body></html>";
            }

            bool success;
            try {
                success = promise.TrySetResult(parsedCode);
            } catch (Exception e) {
                GD.PushError($"Could not resolve authorization promise: {e}.");
                return false;
            }

            if (!success) {
                GD.PushError($"Cannot get authorization code because resolving authorization promise failed.");
                return false;
            }

            return true;
        }

        private static string? ParseAuthorizationCode(string uri) {
            Regex codeRegex;
            try {
                codeRegex = new Regex("code=([a-zA-Z0-9]*)");
            } catch (Exception e) {
                GD.PushWarning($"Could not create code regex: {e}.");
                return null;
            }

            Match match;
            try {
                match = codeRegex.Match(uri);
            } catch (Exception e) {
                GD.PushWarning($"Could not match code regex with uri: {e}.");
                return null;
            }

            if (!match.Success) {
                GD.PushWarning($"Cannot parse authorization code because regex match failed.");
                return null;
            }

            if (match.Groups.Count != 2) {
                GD.PushWarning($"Cannot parse authorization code because regex match groups count is not 2.");
                return null;
            }

            return match.Groups[1].Value;
        }
    }
}
