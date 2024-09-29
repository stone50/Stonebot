namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal class HttpClientWrapper {
        public string RefreshToken => accessToken.RefreshToken;

        public static async Task<HttpClientWrapper?> CreateChatter() {
            Logger.Info("Creating chatter http client wrapper.");
            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var token = await AccessToken.CreateChatter();
            return token is null ? null : new(token);
        }

        public static async Task<HttpClientWrapper?> CreateCollector() {
            Logger.Info("Creating collector http client wrapper.");
            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var token = await AccessToken.CreateCollector();
            return token is null ? null : new(token);
        }

        public async Task<HttpClient?> GetClient() {
            Logger.Info("Getting http client wrapper client.");
            if (cachedClient is not null && !accessToken.IsAboutToExpire) {
                return cachedClient;
            }

            var accessTokenString = await accessToken.GetString();
            if (accessTokenString is null) {
                return null;
            }

            cachedClient = new HttpClient();
            cachedClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenString}");
            cachedClient.DefaultRequestHeaders.Add("Client-Id", accessToken.ClientId);
            return cachedClient;
        }

        public async Task<string?> GetAccessTokenString() => await accessToken.GetString();

        private readonly AccessToken accessToken;
        private HttpClient? cachedClient;

        private HttpClientWrapper(AccessToken accessToken) => this.accessToken = accessToken;
    }
}
