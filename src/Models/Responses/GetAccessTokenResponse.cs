namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal struct GetAccessTokenResponse {
        [JsonPropertyName("access_token")]
        [JsonRequired]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        [JsonRequired]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        [JsonRequired]
        public string RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        [JsonRequired]
        public string[] Scope { get; set; }

        [JsonPropertyName("token_type")]
        [JsonRequired]
        public string TokenType { get; set; }
    }
}
