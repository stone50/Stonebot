namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal class GetAccessTokenResponse {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; init; }

        [JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public required string[] Scope { get; init; }

        [JsonPropertyName("token_type")]
        public required string TokenType { get; init; }
    }
}
