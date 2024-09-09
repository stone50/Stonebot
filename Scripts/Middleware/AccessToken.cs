namespace StoneBot.Scripts.Middleware {
    using Godot;
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    internal static class AccessToken {
        public struct AccessTokenData {
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

        public static async Task<AccessTokenData?> GetAccessTokenData() {
            // TODO: check for stored access token data

            if (Configuration.TwitchStoneBotClientId is null) {
                GD.PushWarning("Cannot get access token because TwitchStoneBotClientId is null.");
                return null;
            }

            if (Configuration.TwitchStoneBotClientSecret is null) {
                GD.PushWarning("Cannot get access token because TwitchStoneBotClientSecret is null.");
                return null;
            }

            if (Configuration.AuthorizationServerPort is null) {
                GD.PushWarning("Cannot get access token because AuthorizationServerPort is null.");
                return null;
            }

            var code = await Authorization.GetAuthorizationCode(new[] { "user:bot", "user:read:chat", "user:write:chat" });
            if (code is null) {
                GD.PushWarning("Cannot get Twitch event sub client because GetAuthorizationCode failed.");
                return null;
            }

            var responseMessage = await TwitchAPI.GetAccessToken(new(), Configuration.TwitchStoneBotClientId, Configuration.TwitchStoneBotClientSecret, code, $"http://localhost:{Configuration.AuthorizationServerPort}");
            if (responseMessage is null || !responseMessage.IsSuccessStatusCode) {
                GD.PushWarning("Cannot get access token because GetAccessToken failed.");
                return null;
            }

            string responseString;
            try {
                responseString = await responseMessage.Content.ReadAsStringAsync();
            } catch (Exception e) {
                GD.PushWarning($"Could not read response string: {e}.");
                return null;
            }

            if (!responseMessage.IsSuccessStatusCode) {
                GD.PushWarning($"Cannot get access token because GetAccessToken failed: {responseString}.");
                return null;
            }

            AccessTokenData accessTokenData;
            try {
                accessTokenData = JsonSerializer.Deserialize<AccessTokenData>(responseString);
            } catch (Exception e) {
                GD.PushWarning($"Could not parse response json: {e}.");
                return null;
            }

            // TODO: store access token data

            return accessTokenData;
        }
    }
}
