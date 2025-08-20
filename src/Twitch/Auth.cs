namespace Stonebot.Twitch {
    using Models.Responses;

    internal static class Auth {
        public static PostDeviceCodeResponse GetDeviceCode() {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/device", new() {
                { "client_id", Config.ClientId },
                { "scopes", Constants.AuthScopes }
            });
            return Utils.SendUnauthorizedPostRequest(url, JsonContext.Default.PostDeviceCodeResponse);
        }

        public static AccessToken GetAccessToken(string deviceCode) {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/token", new() {
                { "client_id", Config.ClientId },
                { "scopes", Constants.AuthScopes },
                { "device_code", deviceCode },
                { "grant_type", "urn:ietf:params:oauth:grant-type:device_code" }
            });
            var accessTokenResponse = Utils.SendUnauthorizedPostRequest(url, JsonContext.Default.GetAccessTokenResponse);
            return ProcessAccessTokenResponse(accessTokenResponse);
        }

        public static AccessToken GetRefreshedAccessToken() {
            var url = Utils.GetUrl("https://id.twitch.tv/oauth2/token", new() {
                { "client_id", Config.ClientId },
                { "grant_type", "refresh_token" },
                { "refresh_token", ProtectedStorage.GetRefreshToken() },
            });
            var accessTokenResponse = Utils.SendUnauthorizedPostRequest(url, JsonContext.Default.GetAccessTokenResponse);
            return ProcessAccessTokenResponse(accessTokenResponse);
        }

        private static AccessToken ProcessAccessTokenResponse(GetAccessTokenResponse accessTokenResponse) {
            ProtectedStorage.SaveRefreshToken(accessTokenResponse.RefreshToken);
            accessTokenResponse.RefreshToken = "";
            var expirationDate = DateTime.UtcNow.AddSeconds(accessTokenResponse.ExpiresIn);
            return new(accessTokenResponse.AccessToken, expirationDate);
        }
    }
}
