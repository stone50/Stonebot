namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator;

    internal static class AuthorizationCode {
        public static async Task<string?> Create(string clientId, string[] scope) {
            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var localhost = IPAddress.Parse("127.0.0.1");
            TcpListener server;
            try {
                server = new(localhost, config.AuthorizationPort);
            } catch (Exception e) {
                GD.PushWarning($"Cannot create authorization code because TcpListener construction failed: {e}.");
                return null;
            }

            try {
                server.Start();
            } catch (Exception e) {
                GD.PushWarning($"Cannot create authorization code because server.Start failed: {e}.");
                return null;
            }

            var state = GetState(32);
            _ = TwitchAPI.Authorize(
                clientId,
                $"http://localhost:{config.AuthorizationPort}",
                scope,
                true,
                state
            );
            var code = await GetCode(server, state);
            try {
                server.Stop();
            } catch { }

            return code;
        }

        private static string GetState(int numChars) {
            var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";
            var state = "";
            for (var i = 0; i < numChars; i++) {
                state += allowedChars[RandomNumberGenerator.GetInt32(allowedChars.Length)];
            }

            return state;
        }

        private static async Task<string?> GetCode(TcpListener server, string state) {
            TcpClient client;
            try {
                client = await server.AcceptTcpClientAsync();
            } catch (Exception e) {
                GD.PushWarning($"Cannot get code because server.AcceptTcpClientAsync failed: {e}.");
                return null;
            }

            try {
                using var stream = client.GetStream();

                var url = await GetUrl(stream);
                if (url is null) {
                    _ = await SendBadRequest(stream);
                    return null;
                }

                if (!GetIsStateValid(url, state)) {
                    _ = await SendBadRequest(stream);
                    return null;
                }

                var code = GetCode(url);
                if (code is null) {
                    _ = await SendBadRequest(stream);
                    return null;
                }

                _ = await SendOkRequest(stream);
                return code;
            } catch (Exception e) {
                GD.PushWarning($"Cannot get code because client.GetStream failed: {e}.");
                return null;
            }
        }

        private static async Task<string?> GetUrl(NetworkStream stream) {
            var buffer = new byte[1024];
            int numBytesRead;
            try {
                numBytesRead = await stream.ReadAsync(buffer);
            } catch (Exception e) {
                GD.PushWarning($"Cannot get url because stream.ReadAsync failed: {e}.");
                return null;
            }

            string message;
            try {
                message = Encoding.Default.GetString(buffer, 0, numBytesRead);
            } catch (Exception e) {
                GD.PushWarning($"Cannot get url because Encoding.Default.GetString failed: {e}.");
                return null;
            }

            var indexOfFirstSpace = message.IndexOf(' ');
            if (indexOfFirstSpace == -1) {
                GD.PushWarning("Cannot get url because indexOfFirstSpace is -1.");
                return null;
            }

            var indexOfSecondSpace = message.IndexOf(' ', indexOfFirstSpace + 1);
            if (indexOfSecondSpace == -1) {
                GD.PushWarning("Cannot get url because indexOfSecondSpace is -1.");
                return null;
            }

            string url;
            try {
                url = message.Substring(indexOfFirstSpace + 1, indexOfSecondSpace - indexOfFirstSpace);
            } catch (Exception e) {
                GD.PushWarning($"Cannot get url because message.Substring failed: {e}.");
                return null;
            }

            return url;
        }

        private static bool GetIsStateValid(string url, string state) {
            var stateRegex = new Regex("state=([a-zA-Z0-9_.\\-~]*)");
            var match = stateRegex.Match(url);
            if (!match.Success) {
                GD.PushWarning("Cannot get is state valid because match.Success is false.");
                return false;
            }

            if (match.Groups.Count != 2) {
                GD.PushWarning("Cannot get is state valid because match.Groups.Count is not 2.");
                return false;
            }

            return match.Groups[1].Value == state;
        }

        private static string? GetCode(string url) {
            var codeRegex = new Regex("code=([a-zA-Z0-9]*)");
            var match = codeRegex.Match(url);
            if (!match.Success) {
                GD.PushWarning($"Cannot get code becausematch.Success is false.");
                return null;
            }

            if (match.Groups.Count != 2) {
                GD.PushWarning($"Cannot get code because match.Groups.Count is not 2.");
                return null;
            }

            return match.Groups[1].Value;
        }

        private static async Task<bool> SendBadRequest(NetworkStream stream) {
            try {
                await stream.WriteAsync(Encoding.Default.GetBytes("HTTP/1.1 400 Bad Request\r\n\r\n<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>Please check the logs to see why authorization failed.</p></body></html>"));
            } catch (Exception e) {
                GD.PushWarning($"Cannot send bad request because stream.WriteAsync failed: {e}.");
                return false;
            }

            return true;
        }

        private static async Task<bool> SendOkRequest(NetworkStream stream) {
            try {
                await stream.WriteAsync(Encoding.Default.GetBytes("HTTP/1.1 200 OK\r\n\r\n<html><head><title>Authorization Succeeded</title></head><body><h1>Authorization Success! :)</h1><p>You can close this tab.</p></body></html>"));
            } catch (Exception e) {
                GD.PushWarning($"Cannot send ok request because stream.WriteAsync failed: {e}.");
                return false;
            }

            return true;
        }
    }
}
