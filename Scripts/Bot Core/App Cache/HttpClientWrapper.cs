namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using System.Text.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal class HttpClientWrapper {
        public string RefreshToken => accessToken.RefreshToken;

        public string MaskedSerialized => JsonSerializer.Serialize(new {
            RefreshToken = Scripts.Util.GetMasked(RefreshToken),
        });

        public static async Task<HttpClientWrapper?> CreateChatter() {
            var logPrefix = $"{nameof(HttpClientWrapper)} | {nameof(CreateChatter)}";
            Logger.Info(logPrefix);

            var token = await AccessToken.CreateChatter();
            if (token is null) {
                Logger.Warning($"{logPrefix} | {nameof(AccessToken.CreateChatter)} result is null.");
                return null;
            }

            return new(token);
        }

        public static async Task<HttpClientWrapper?> CreateCollector() {
            var logPrefix = $"{nameof(HttpClientWrapper)} | {nameof(CreateCollector)}";
            Logger.Info(logPrefix);

            var token = await AccessToken.CreateCollector();
            if (token is null) {
                Logger.Warning($"{logPrefix} | {nameof(AccessToken.CreateCollector)} result is null.");
                return null;
            }

            return new(token);
        }

        public async Task<HttpClient?> GetClient() {
            var logPrefix = $"{nameof(HttpClientWrapper)} | {nameof(GetClient)}";
            Logger.Info(logPrefix);

            if (cachedClient is not null && !accessToken.IsAboutToExpire) {
                return cachedClient;
            }

            var accessTokenString = await accessToken.GetString();
            if (accessTokenString is null) {
                Logger.Warning($"{logPrefix} | {nameof(accessToken.GetString)} result is null.");
                return null;
            }

            cachedClient = new HttpClient();
            cachedClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenString}");
            cachedClient.DefaultRequestHeaders.Add("Client-Id", accessToken.ClientId);
            return cachedClient;
        }

        public async Task<string?> GetAccessTokenString() {
            var logPrefix = $"{nameof(HttpClientWrapper)} | {nameof(GetAccessTokenString)}";
            Logger.Info(logPrefix);

            var accessTokenString = await accessToken.GetString();
            if (accessTokenString is null) {
                Logger.Warning($"{logPrefix} | {nameof(accessToken.GetString)} result is null.");
                return null;
            }

            return accessTokenString;
        }

        private readonly AccessToken accessToken;
        private HttpClient? cachedClient;

        private HttpClientWrapper(AccessToken accessToken) {
            Logger.Info($"{nameof(HttpClientWrapper)} | Constructor\n{nameof(accessToken)}: {accessToken.MaskedSerialized}");

            this.accessToken = accessToken;
        }
    }
}
