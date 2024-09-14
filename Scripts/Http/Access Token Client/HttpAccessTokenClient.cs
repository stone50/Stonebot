namespace StoneBot.Scripts.Http.Access_Token_Client {
    using Access_Token;
    using App_Cache;
    using Godot;
    using Models;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal abstract class HttpAccessTokenClient {
        protected AccessToken AccessToken;

        public HttpAccessTokenClient(AccessToken accessToken) => AccessToken = accessToken;

        public async Task<HttpClient?> GetClient() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot get access token client because configValues is null.");
                return null;
            }

            var configValues = (ConfigValues)potentialConfigValues;
            var accessTokenString = await AccessToken.GetString();
            if (accessTokenString is null) {
                GD.PushWarning("Cannot get access token client because AccessToken.GetString failed.");
                return null;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenString}");
            client.DefaultRequestHeaders.Add("Client-Id", configValues.BotClientId);
            return client;
        }
    }
}
