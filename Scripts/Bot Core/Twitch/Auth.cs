namespace Stonebot.Scripts.Bot_Core.Twitch {
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        public static Process? Authorize(string clientId, string redirectUri, string[] scope, bool forceVerify = false, string? state = null) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(Authorize)}";
            Logger.Info($"{logPrefix}\n{nameof(clientId)}: {clientId}\n{nameof(redirectUri)}: {redirectUri}\n{nameof(scope)}: {JsonSerializer.Serialize(scope)}\n{nameof(forceVerify)}: {forceVerify}\n{nameof(state)}: {state}");

            var scopeParam = string.Join(" ", scope);
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&force_verify={(forceVerify ? "true" : "false")}&redirect_uri={redirectUri}&response_type=code&scope={scopeParam}";
            if (state is not null) {
                process.StartInfo.FileName += $"&state={state}";
            }

            try {
                if (!process.Start()) {
                    Logger.Warning($"{logPrefix} | {nameof(process.Start)} result is false.");
                    return null;
                }
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(process.Start)} threw: {e}.");
                return null;
            }

            return process;
        }

        // no access token
        public static async Task<HttpResponseMessage?> GetAccessToken(HttpClient client, string clientId, string clientSecret, string authorizationCode, string redirectUri) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(GetAccessToken)}";
            Logger.Info($"{logPrefix}\n{nameof(client)}: {client}\n{nameof(clientId)}: {clientId}\n{nameof(clientSecret)}: {Scripts.Util.GetMasked(clientSecret)}\n{nameof(authorizationCode)}: {authorizationCode}\n{nameof(redirectUri)}: {redirectUri}");

            var requestUri = $"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&code={authorizationCode}&grant_type=authorization_code&redirect_uri={redirectUri}";
            HttpContent? content = null;
            try {
                return await client.PostAsync(requestUri, content);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(client.PostAsync)} threw: {e}.\n{nameof(content)}: {content}");
                return null;
            }
        }

        // no access token
        public static async Task<HttpResponseMessage?> RefreshAccessToken(HttpClient client, string clientId, string clientSecret, string refreshToken) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(RefreshAccessToken)}";
            Logger.Info($"{logPrefix}\n{nameof(client)}: {client}\n{nameof(clientId)}: {clientId}\n{nameof(clientSecret)}: {Scripts.Util.GetMasked(clientSecret)}\n{nameof(refreshToken)}: {Scripts.Util.GetMasked(refreshToken)}");

            var requestUri = $"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=refresh_token&refresh_token={refreshToken}";
            HttpContent? content = null;
            try {
                return await client.PostAsync(requestUri, content);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(client.PostAsync)} threw: {e}.\n{nameof(content)}: {content}");
                return null;
            }
        }
    }
}
