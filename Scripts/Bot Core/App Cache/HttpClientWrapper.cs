namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal class HttpClientWrapper {
        public string RefreshToken => accessToken.RefreshToken;

        public static async Task<HttpClientWrapper?> CreateChatter() {
            Logger.Info("Creating chatter http client wrapper.");

            var token = await AccessToken.CreateChatter();
            if (token is null) {
                Logger.Warning("Could not create chatter http client wrapper because access token create chatter attempt failed.");
                return null;
            }

            return new(token);
        }

        public static async Task<HttpClientWrapper?> CreateCollector() {
            Logger.Info("Creating collector http client wrapper.");

            var token = await AccessToken.CreateCollector();
            if (token is null) {
                Logger.Warning("Could not create collector http client wrapper because access token create collector attempt failed.");
                return null;
            }

            return new(token);
        }

        public async Task<HttpClient?> GetClient() {
            Logger.Info("Getting http client wrapper client.");

            if (cachedClient is not null && !accessToken.IsAboutToExpire) {
                return cachedClient;
            }

            var accessTokenString = await accessToken.GetString();
            if (accessTokenString is null) {
                Logger.Warning("Could not get http client wrapper client because access token get string attempt failed.");
                return null;
            }

            cachedClient = new HttpClient();
            cachedClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenString}");
            cachedClient.DefaultRequestHeaders.Add("Client-Id", accessToken.ClientId);
            return cachedClient;
        }

        public async Task<string?> GetAccessTokenString() {
            Logger.Info("Getting http client wrapper access token string.");

            var accessTokenString = await accessToken.GetString();
            if (accessTokenString is null) {
                Logger.Warning("Could not get http client wrapper access token string because access token get string attempt failed.");
                return null;
            }

            return accessTokenString;
        }

        private readonly AccessToken accessToken;
        private HttpClient? cachedClient;

        private HttpClientWrapper(AccessToken accessToken) => this.accessToken = accessToken;
    }
}
