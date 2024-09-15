namespace StoneBot.Scripts.Bot_Core.App_Cache.Value_Getters {
    using Godot;
    using Models;
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator;
    using TcpServer = Http.TcpServer;

    internal static partial class ValueGetters {
        public static async Task<string?> GetAuthorizationCode() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot get authorization code because configValues is null.");
                return null;
            }

            var configValues = (ConfigValues)potentialConfigValues;
            var localhost = IPAddress.Parse("127.0.0.1");
            TcpServer authorizationServer;
            try {
                authorizationServer = new TcpServer(localhost, configValues.AuthorizationPort);
            } catch (Exception e) {
                GD.PushWarning($"Could not create server: {e}.");
                return null;
            }

            if (!authorizationServer.Start()) {
                GD.PushWarning($"Cannot get authorization code because server start failed.");
                return null;
            }

            var promise = new TaskCompletionSource<string?>();
            var state = GetNewState(32);
            authorizationServer.RequestHandler += (sender, args) => {
                if (!HandleAuthorizationServerRequest(args, promise, state)) {
                    GD.PushWarning("Cannot get authorization code because HandleAuthorizationServerRequest failed.");
                    if (!authorizationServer.Stop()) {
                        GD.PushError("Authorization server stop failed.");
                    }

                    return;
                }
            };

            var authorizationProcess = TwitchAPI.Authorize(
                configValues.BotClientId,
                $"http://localhost:{configValues.AuthorizationPort}",
                new[] { "user:bot", "user:read:chat", "user:write:chat" },
                false,
                state
            );

            if (authorizationProcess is null) {
                GD.PushWarning("Cannot get authorization code because authorization process failed.");
                if (!authorizationServer.Stop()) {
                    GD.PushError("Authorization server stop failed.");
                }

                return null;
            }

            var result = await promise.Task;

            if (!authorizationServer.Stop()) {
                GD.PushError("Authorization server stop failed.");
                // No need to return null here. Although the server failed to stop,
                // we got the data we needed, and the server will be reclaimed anyway.
            }

            return result;
        }

        private static string GetNewState(int numChars) {
            var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";
            var state = "";
            for (var i = 0; i < numChars; i++) {
                state += allowedChars[RandomNumberGenerator.GetInt32(allowedChars.Length)];
            }

            return state;
        }

        private static bool HandleAuthorizationServerRequest(TcpServer.HttpRequestHandlerArgs args, TaskCompletionSource<string?> promise, string state) {
            string? parsedCode = null;
            if (ValidateAuthorizationState(args.Request.Uri, state)) {
                parsedCode = GetAuthorizationCodeFromUri(args.Request.Uri);
                if (parsedCode is null) {
                    GD.PushWarning("Cannot get authorization code because authorization code could not be parsed from request uri.");
                    args.Response = new() {
                        StatusCode = 400,
                        ReasonPhrase = "Bad Request",
                        Message = "<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>Cannot get authorization code because authorization code could not be parsed from request uri.</p></body></html>"
                    };
                } else {
                    args.Response.Message = "<html><head><title>Authorization Succeeded</title></head><body><h1>Authorization Success! :)</h1><p>You can close this tab.</p></body></html>";
                }
            } else {
                GD.PushWarning("Cannot get authorization code because authorization state could not be validated.");
                args.Response = new() {
                    StatusCode = 400,
                    ReasonPhrase = "Bad Request",
                    Message = "<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>Cannot get authorization code because authorization state could not be validated.</p></body></html>"
                };
            }

            if (!promise.TrySetResult(parsedCode)) {
                GD.PushError($"Cannot get authorization code because resolving authorization promise failed.");
                return false;
            }

            return true;
        }

        private static bool ValidateAuthorizationState(string uri, string state) {
            var stateRegex = new Regex("state=([a-zA-Z0-9_.\\-~]*)");
            var match = stateRegex.Match(uri);
            if (!match.Success) {
                GD.PushWarning($"Cannot validate authorization state because regex match failed.");
                return false;
            }

            if (match.Groups.Count != 2) {
                GD.PushWarning($"Cannot validate authorization state because regex match groups count is not 2.");
                return false;
            }

            return match.Groups[1].Value == state;
        }

        private static string? GetAuthorizationCodeFromUri(string uri) {
            var codeRegex = new Regex("code=([a-zA-Z0-9]*)");
            var match = codeRegex.Match(uri);
            if (!match.Success) {
                GD.PushWarning($"Cannot get authorization code because regex match failed.");
                return null;
            }

            if (match.Groups.Count != 2) {
                GD.PushWarning($"Cannot get authorization code because regex match groups count is not 2.");
                return null;
            }

            return match.Groups[1].Value;
        }
    }
}
