namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Twitch;
    using RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator;

    internal static partial class AuthorizationCode {
        public static async Task<string?> Create(string clientId, string[] scope) {
            Logger.Info("Creating authorization code.");

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not create authorization code because config get attempt failed.");
                return null;
            }

            var localhost = IPAddress.Parse("127.0.0.1");
            TcpListener server;
            try {
                server = new(localhost, config.AuthorizationPort);
            } catch (Exception e) {
                Logger.Warning($"Could not create authorization code because TcpListener construct attempt failed: {e}. Found: {config.AuthorizationPort}.");
                return null;
            }

            try {
                server.Start();
            } catch (Exception e) {
                Logger.Warning($"Could not create authorization code because server start attempt failed: {e}.");
                return null;
            }

            var state = GetState(32);
            var authorizationProcess = TwitchAPI.Authorize(
                clientId,
                $"http://localhost:{config.AuthorizationPort}",
                scope,
                true,
                state
            );
            if (authorizationProcess is null) {
                Logger.Warning("Could not create authorization code because Twitch authorize attempt failed.");
                return null;
            }

            var code = await GetCode(server, state);
            try {
                server.Stop();
            } catch (Exception e) {
                Logger.Warning($"Server stop attempt failed: {e}.");
            }

            if (code is null) {
                Logger.Warning("Could not create authorization code because get code attempt failed.");
                return null;
            }

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
                Logger.Warning($"Could not get authorization code because server accept client attempt failed: {e}.");
                return null;
            }

            try {
                using var stream = client.GetStream();

                var url = await GetUrl(stream);
                if (url is null) {
                    if (!await SendBadRequest(stream)) {
                        Logger.Warning("Send bad request attempt failed.");
                    }

                    Logger.Warning("Could not get authorization code because get url attempt failed.");
                    return null;
                }

                if (!GetIsStateValid(url, state)) {
                    if (!await SendBadRequest(stream)) {
                        Logger.Warning("Send bad request attempt failed.");
                    }

                    Logger.Warning("Could not get authorization code because get is valid state attempt failed.");
                    return null;
                }

                var code = GetCodeFromUrl(url);
                if (code is null) {
                    if (!await SendBadRequest(stream)) {
                        Logger.Warning("Send bad request attempt failed.");
                    }

                    Logger.Warning($"Could not get authorization code because get code from url attempt failed. Found: {url}.");
                    return null;
                }

                if (!await SendOkRequest(stream)) {
                    Logger.Warning("Send ok request attempt failed.");
                }

                return code;
            } catch (Exception e) {
                Logger.Warning($"Could not get authorization code because client get stream attempt failed: {e}.");
                return null;
            }
        }

        private static async Task<string?> GetUrl(NetworkStream stream) {
            var buffer = new byte[1024];
            int numBytesRead;
            try {
                numBytesRead = await stream.ReadAsync(buffer);
            } catch (Exception e) {
                Logger.Warning($"Could not get url because stream read attempt failed: {e}.");
                return null;
            }

            string message;
            try {
                message = Encoding.Default.GetString(buffer, 0, numBytesRead);
            } catch (Exception e) {
                Logger.Warning($"Could not get url because encoding default get string attempt failed: {e}.");
                return null;
            }

            void LogParsingWarning() => Logger.Warning($"Could not get url because message could not be parsed. Found: {message}.");
            var indexOfFirstSpace = message.IndexOf(' ');
            if (indexOfFirstSpace == -1) {
                LogParsingWarning();
                return null;
            }

            int indexOfSecondSpace;
            try {
                indexOfSecondSpace = message.IndexOf(' ', indexOfFirstSpace + 1);
            } catch {
                LogParsingWarning();
                return null;
            }

            if (indexOfSecondSpace == -1) {
                LogParsingWarning();
                return null;
            }

            string url;
            try {
                url = message.Substring(indexOfFirstSpace + 1, indexOfSecondSpace - indexOfFirstSpace);
            } catch {
                LogParsingWarning();
                return null;
            }

            return url;
        }

        private static bool GetIsStateValid(string url, string state) {
            var stateRegex = StateRegex();
            var match = stateRegex.Match(url);
            void LogMatchWarning() => Logger.Warning($"Could not get is state valid because state regex match attempt failed. Found: {url}.");
            if (!match.Success) {
                LogMatchWarning();
                return false;
            }

            if (match.Groups.Count != 2) {
                LogMatchWarning();
                return false;
            }

            return match.Groups[1].Value == state;
        }

        private static string? GetCodeFromUrl(string url) {
            var codeRegex = CodeRegex();
            var match = codeRegex.Match(url);
            void LogMatchWarning() => Logger.Warning($"Could not get authorization code from url because code regex match attempt failed. Found: {url}.");
            if (!match.Success) {
                LogMatchWarning();
                return null;
            }

            if (match.Groups.Count != 2) {
                LogMatchWarning();
                return null;
            }

            return match.Groups[1].Value;
        }

        private static async Task<bool> SendBadRequest(NetworkStream stream) {
            try {
                await stream.WriteAsync(Encoding.Default.GetBytes("HTTP/1.1 400 Bad Request\r\n\r\n<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>Please check the logs to see why authorization failed.</p></body></html>"));
            } catch (Exception e) {
                Logger.Warning($"Could not send bad request because stream write attempt failed: {e}.");
                return false;
            }

            return true;
        }

        private static async Task<bool> SendOkRequest(NetworkStream stream) {
            try {
                await stream.WriteAsync(Encoding.Default.GetBytes("HTTP/1.1 200 OK\r\n\r\n<html><head><title>Authorization Succeeded</title></head><body><h1>Authorization Success! :)</h1><p>You can close this tab.</p></body></html>"));
            } catch (Exception e) {
                Logger.Warning($"Could not send ok request because stream write attempt failed: {e}.");
                return false;
            }

            return true;
        }

        [GeneratedRegex("state=([a-zA-Z0-9_.\\-~]*)")]
        private static partial Regex StateRegex();

        [GeneratedRegex("code=([a-zA-Z0-9]*)")]
        private static partial Regex CodeRegex();
    }
}
