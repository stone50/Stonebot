namespace Stonebot {
    using Twitch;

    internal static class Cache {
        public static readonly HttpClient DefaultHttpClient = new();
        public static AccessToken AccessToken => GetAccessToken();
        public static HttpClient AuthorizedHttpClient => GetAuthorizedHttpClient();
        public static string BroadcasterId => GetBroadcasterId();
        public static string ChatterId => GetChatterId();
        public static string ChatterDisplayName => GetChatterDisplayName();

        public static void LoadNewAccessToken(string deviceCode) => accessToken = Auth.GetAccessToken(deviceCode);

        private static AccessToken GetAccessToken() {
            if (accessToken == null || GetAccessTokenShouldBeRefreshed()) {
                accessToken = Auth.GetRefreshedAccessToken();
            }

            return (AccessToken)accessToken!;
        }

        private static HttpClient GetAuthorizedHttpClient() {
            if (authorizedHttpClient == null) {
                authorizedHttpClient = new HttpClient();
                authorizedHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken.Value}");
                authorizedHttpClient.DefaultRequestHeaders.Add("Client-Id", Config.ClientId);
                return authorizedHttpClient;
            }

            if (GetAccessTokenShouldBeRefreshed()) {
                accessToken = Auth.GetRefreshedAccessToken();
                _ = authorizedHttpClient.DefaultRequestHeaders.Remove("Authorization");
                authorizedHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken.Value}");
            }

            return authorizedHttpClient;
        }

        private static string GetBroadcasterId() {
            broadcasterId ??= User.GetBroadcaster().Id;
            return broadcasterId;
        }

        private static string GetChatterId() {
            if (chatterId == null) {
                var chatter = User.GetChatter();
                chatterId = chatter.Id;
                chatterDisplayName = chatter.DisplayName;
            }

            return chatterId;
        }

        private static string GetChatterDisplayName() {
            if (chatterDisplayName == null) {
                var chatter = User.GetChatter();
                chatterDisplayName = chatter.DisplayName;
                chatterId = chatter.Id;
            }

            return chatterDisplayName;
        }

        private static AccessToken? accessToken;
        private static HttpClient? authorizedHttpClient;
        private static string? broadcasterId;
        private static string? chatterId;
        private static string? chatterDisplayName;

        private static bool GetAccessTokenShouldBeRefreshed() => DateTime.UtcNow.AddSeconds(Constants.AccessTokenExpirationMarginSecs) > ((AccessToken)accessToken!).ExpirationDate;
    }
}
