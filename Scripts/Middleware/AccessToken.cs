namespace StoneBot.Scripts.Middleware {
    using Godot;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    internal static class AccessToken {
        public struct UserAccessTokenData {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }
            [JsonPropertyName("scope")]
            public string[] Scope { get; set; }
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
        }

        public struct AppAccessTokenData {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
        }

        public static async Task<UserAccessTokenData?> GetUserAccessTokenData() {
            // TODO: check for stored access token data

            if (Configuration.TwitchStoneBotClientId is null) {
                GD.PushWarning("Cannot get user access token because TwitchStoneBotClientId is null.");
                return null;
            }

            if (Configuration.TwitchStoneBotClientSecret is null) {
                GD.PushWarning("Cannot get user access token because TwitchStoneBotClientSecret is null.");
                return null;
            }

            if (Configuration.AuthorizationServerPort is null) {
                GD.PushWarning("Cannot get user access token because AuthorizationServerPort is null.");
                return null;
            }

            var code = await Authorization.GetAuthorizationCode(new[] { "user:bot", "user:read:chat", "user:write:chat" });
            if (code is null) {
                GD.PushWarning("Cannot get user access token because GetAuthorizationCode failed.");
                return null;
            }

            var accessTokenData = await Util.ProcessHttpResponseMessage<UserAccessTokenData>(
                await TwitchAPI.GetUserAccessToken(
                    new(),
                    Configuration.TwitchStoneBotClientId,
                    Configuration.TwitchStoneBotClientSecret,
                    code,
                    $"http://localhost:{Configuration.AuthorizationServerPort}"
                )
            );
            if (accessTokenData is null) {
                GD.PushWarning("Cannot get user access token because ProcessHttpResponseMessage failed.");
                return null;
            }

            // TODO: store access token data

            return accessTokenData;
        }

        public static async Task<AppAccessTokenData?> GetAppAccessTokenData() {
            if (Configuration.TwitchStoneBotClientId is null) {
                GD.PushWarning("Cannot get app access token because TwitchStoneBotClientId is null.");
                return null;
            }

            if (Configuration.TwitchStoneBotClientSecret is null) {
                GD.PushWarning("Cannot get app access token because TwitchStoneBotClientSecret is null.");
                return null;
            }

            var accessTokenData = await Util.ProcessHttpResponseMessage<AppAccessTokenData>(
                await TwitchAPI.GetAppAccessToken(
                    new(),
                    Configuration.TwitchStoneBotClientId,
                    Configuration.TwitchStoneBotClientSecret
                )
            );
            if (accessTokenData is null) {
                GD.PushWarning("Cannot get app access token because ProcessHttpResponseMessage failed.");
                return null;
            }

            return accessTokenData;
        }
    }
}
