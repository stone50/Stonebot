namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Twitch;
    using RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator;

    internal static partial class AuthorizationCode {
        public static async Task<string?> Create(string clientId, string[] scope) {
            var logPrefix = $"{nameof(AuthorizationCode)} | {nameof(Create)}";
            Logger.Info($"{logPrefix}\n{nameof(clientId)}: {clientId}\n{nameof(scope)}: {JsonSerializer.Serialize(scope)}");

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return null;
            }

            var localhost = IPAddress.Parse("127.0.0.1");
            TcpListener server;
            try {
                server = new(localhost, config.AuthorizationPort);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(TcpListener)} Constructor threw: {e}.\n{nameof(localhost)}: {localhost}\n{nameof(config.AuthorizationPort)}: {config.AuthorizationPort}");
                return null;
            }

            try {
                server.Start();
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(server.Start)} threw: {e}.\n{nameof(server)}: {server}");
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
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.Authorize)} result is null.");
                return null;
            }

            var code = await GetCode(server, state);
            try {
                server.Stop();
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(server.Stop)} threw: {e}.\n{nameof(server)}: {server}");
            }

            if (code is null) {
                Logger.Warning($"{logPrefix} | {nameof(GetCode)} result is null.");
                return null;
            }

            return code;
        }

        private static string GetState(int numChars) {
            Logger.Info($"{nameof(AuthorizationCode)} | {nameof(GetState)}\n{nameof(numChars)}: {numChars}");

            var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";
            var state = "";
            for (var i = 0; i < numChars; i++) {
                state += allowedChars[RandomNumberGenerator.GetInt32(allowedChars.Length)];
            }

            return state;
        }

        private static async Task<string?> GetCode(TcpListener server, string state) {
            var logPrefix = $"{nameof(AuthorizationCode)} | {nameof(GetCode)}";
            Logger.Info($"{logPrefix} | \n{nameof(server)}: {server}\n{nameof(state)}: {state}");

            TcpClient client;
            try {
                client = await server.AcceptTcpClientAsync();
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(server.AcceptTcpClientAsync)} threw: {e}.");
                return null;
            }

            try {
                using var stream = client.GetStream();
                async Task DoSendBadRequest() {
                    if (!await SendBadRequest(stream)) {
                        Logger.Warning($"{logPrefix} | {nameof(SendBadRequest)} result is false.");
                    }
                }

                var url = await GetUrl(stream);
                if (url is null) {
                    await DoSendBadRequest();
                    Logger.Warning($"{logPrefix} | {nameof(GetUrl)} result is null.");
                    return null;
                }

                if (!GetIsStateValid(url, state)) {
                    await DoSendBadRequest();
                    Logger.Warning($"{logPrefix} | {nameof(GetIsStateValid)} result is false.");
                    return null;
                }

                var code = GetCodeFromUrl(url);
                if (code is null) {
                    await DoSendBadRequest();
                    Logger.Warning($"{logPrefix} | {nameof(GetCodeFromUrl)} result is null.");
                    return null;
                }

                if (!await SendOkRequest(stream)) {
                    Logger.Warning($"{logPrefix} | {nameof(SendOkRequest)} result is false.");
                }

                return code;
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(client.GetStream)} threw: {e}.\n{nameof(client)}: {client}");
                return null;
            }
        }

        private static async Task<string?> GetUrl(NetworkStream stream) {
            var logPrefix = $"{nameof(AuthorizationCode)} | {nameof(GetUrl)}";
            Logger.Info($"{logPrefix}\n{nameof(stream)}: {stream}");

            var buffer = new byte[1024];
            int numBytesRead;
            try {
                numBytesRead = await stream.ReadAsync(buffer);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(stream.ReadAsync)} threw: {e}.\n{nameof(buffer)}: {JsonSerializer.Serialize(buffer)}");
                return null;
            }

            var index = 0;
            string message;
            try {
                message = Encoding.Default.GetString(buffer, index, numBytesRead);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(Encoding.Default.GetString)} threw: {e}.\n{nameof(buffer)}: {JsonSerializer.Serialize(buffer)}\n{nameof(index)}: {index}\n{nameof(numBytesRead)}: {numBytesRead}");
                return null;
            }

            var spaceChar = ' ';
            var indexOfFirstSpace = message.IndexOf(spaceChar);
            if (indexOfFirstSpace == -1) {
                Logger.Warning($"{logPrefix} | {nameof(message.IndexOf)} result is -1.\n{nameof(message)}: {message}\n{nameof(spaceChar)}: {spaceChar}");
                return null;
            }

            var startIndex = indexOfFirstSpace + 1;
            int indexOfSecondSpace;
            try {
                indexOfSecondSpace = message.IndexOf(spaceChar, startIndex);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(message.IndexOf)} threw: {e}.\n{nameof(message)}: {message}\n{nameof(spaceChar)}: {spaceChar}\n{nameof(startIndex)}: {startIndex}");
                return null;
            }

            if (indexOfSecondSpace == -1) {
                Logger.Warning($"{logPrefix} | {nameof(message.IndexOf)} result is -1.\n{nameof(message)}: {message}\n{nameof(spaceChar)}: {spaceChar}\n{nameof(startIndex)}: {startIndex}");
                return null;
            }

            var length = indexOfSecondSpace - indexOfFirstSpace;
            string url;
            try {
                url = message.Substring(startIndex, length);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(message.Substring)} threw: {e}.\n{nameof(message)}: {message}\n{nameof(startIndex)}: {startIndex}\n{nameof(length)}: {length}");
                return null;
            }

            return url;
        }

        private static bool GetIsStateValid(string url, string state) {
            Logger.Info($"{nameof(AuthorizationCode)} | {nameof(GetIsStateValid)}\n{nameof(url)}: {url}\n{nameof(state)}: {state}");

            var match = StateRegex().Match(url);
            return match.Success && match.Groups.Count == 2 && match.Groups[1].Value == state;
        }

        private static string? GetCodeFromUrl(string url) {
            var logPrefix = $"{nameof(AuthorizationCode)} | {nameof(GetCodeFromUrl)}";
            Logger.Info($"{logPrefix}\n{nameof(url)}: {url}");

            var codeRegex = CodeRegex();
            var match = codeRegex.Match(url);
            if (!match.Success) {
                Logger.Warning($"{logPrefix} | {nameof(codeRegex.Match)} was unsuccessful.\n{nameof(codeRegex)}: {codeRegex}");
                return null;
            }

            if (match.Groups.Count != 2) {
                Logger.Warning($"{logPrefix} | {nameof(codeRegex.Match)} was unsuccessful.\n{nameof(codeRegex)}: {codeRegex}");
                return null;
            }

            return match.Groups[1].Value;
        }

        private static async Task<bool> SendBadRequest(NetworkStream stream) {
            var logPrefix = $"{nameof(AuthorizationCode)} | {nameof(SendBadRequest)}";
            Logger.Info($"{logPrefix}\n{nameof(stream)}: {stream}");

            var buffer = Encoding.Default.GetBytes("HTTP/1.1 400 Bad Request\r\n\r\n<html><head><title>Authorization Failed</title></head><body><h1>:(</h1><p>Please check the logs to see why authorization failed.</p></body></html>");
            try {
                await stream.WriteAsync(buffer);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(stream.WriteAsync)} threw: {e}.\n{nameof(buffer)}: {JsonSerializer.Serialize(buffer)}");
                return false;
            }

            return true;
        }

        private static async Task<bool> SendOkRequest(NetworkStream stream) {
            var logPrefix = $"{nameof(AuthorizationCode)} | {nameof(SendOkRequest)}";
            Logger.Info($"{logPrefix}\n{nameof(stream)}: {stream}");

            var buffer = Encoding.Default.GetBytes("HTTP/1.1 200 OK\r\n\r\n<html><head><title>Authorization Succeeded</title></head><body><h1>Authorization Success! :)</h1><p>You can close this tab.</p></body></html>");
            try {
                await stream.WriteAsync(buffer);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(stream.WriteAsync)} threw: {e}.\n{nameof(buffer)}: {JsonSerializer.Serialize(buffer)}");
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
