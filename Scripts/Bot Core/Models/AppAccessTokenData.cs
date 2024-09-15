namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    public struct AppAccessTokenData {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
