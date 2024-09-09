namespace StoneBot.Scripts.Middleware {
    using Godot;
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator;
    using TcpServer = TcpServer; // why is this necessary??

    internal static class Authorization {
        public static async Task<string?> GetAuthorizationCode(string[] scope) {
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

            var state = GetNewState(32);
            if (state is null) {
                GD.PushWarning("Cannot get authorization code because GetNewState failed.");
                return null;
            }

            authorizationServer.RequestHandler += (sender, args) => {
                var success = HandleAuthorizationServerRequest(args, promise, state);
                if (!success) {
                    GD.PushWarning("Cannot get authorization code because HandleAuthorizationServerRequest failed.");
                    _ = authorizationServer.Stop();
                    return;
                }
            };

            var authorizationProcess = TwitchAPI.Authorize(Configuration.TwitchStoneBotClientId, $"http://localhost:{Configuration.AuthorizationServerPort}", scope, false, state);

            if (authorizationProcess is null) {
                GD.PushWarning("Cannot get authorization code because authorization process failed.");
                _ = authorizationServer.Stop();
                return null;
            }

            var result = await promise.Task;

            var success = authorizationServer.Stop();
            if (!success) {
                GD.PushWarning("Cannot get authorization code because server stop failed.");
                return null;
            }

            return result;
        }

        private static string? GetNewState(int numChars) {
            var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";
            var state = "";
            for (var i = 0; i < numChars; i++) {
                try {
                    state += allowedChars[RandomNumberGenerator.GetInt32(allowedChars.Length)];
                } catch (Exception e) {
                    GD.PushWarning($"Could not get random int: {e}.");
                    return null;
                }
            }

            return state;
        }

        private static bool HandleAuthorizationServerRequest(TcpServer.HttpRequestHandlerArgs args, TaskCompletionSource<string?> promise, string state) {
            string? parsedCode = null;
            var isStateValid = ValidateAuthorizationState(args.Request.URI, state);
            if (!isStateValid) {
                GD.PushWarning("Cannot get authorization code because authorization state could not be validated.");
                args.Response = new() {
                    StatusCode = 400,
                    ReasonPhrase = "Bad Request",
                    Message = "<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>Cannot get authorization code because authorization state could not be validated.</p></body></html>"
                };
            } else {
                parsedCode = ParseAuthorizationCode(args.Request.URI);
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

        private static bool ValidateAuthorizationState(string uri, string state) {
            Regex stateRegex;
            try {
                stateRegex = new Regex("state=([a-zA-Z0-9_.\\-~]*)");
            } catch (Exception e) {
                GD.PushWarning($"Could not create code regex: {e}.");
                return false;
            }

            Match match;
            try {
                match = stateRegex.Match(uri);
            } catch (Exception e) {
                GD.PushWarning($"Could not match code regex with uri: {e}.");
                return false;
            }

            if (!match.Success) {
                GD.PushWarning($"Cannot parse authorization code because regex match failed.");
                return false;
            }

            if (match.Groups.Count != 2) {
                GD.PushWarning($"Cannot parse authorization code because regex match groups count is not 2.");
                return false;
            }

            return match.Groups[1].Value == state;
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
