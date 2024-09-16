namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal class HttpClientWrapper {
        public static async Task<HttpClientWrapper?> Create() {
            var accessToken = await AppCache.AccessToken.Get();
            return accessToken is null ? null : new(accessToken);
        }

        public async Task<HttpClient?> GetClient() {
            if (cachedClient is not null && !accessToken.IsAboutToExpire) {
                return cachedClient;
            }

            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var accessTokenString = await accessToken.GetString();
            if (accessTokenString is null) {
                return null;
            }

            cachedClient = new HttpClient();
            cachedClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenString}");
            cachedClient.DefaultRequestHeaders.Add("Client-Id", config.BotClientId);
            return cachedClient;
        }

        private readonly AccessToken accessToken;
        private HttpClient? cachedClient;

        private HttpClientWrapper(AccessToken token) => accessToken = token;
    }
}
