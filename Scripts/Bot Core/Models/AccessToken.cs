namespace Stonebot.Scripts.Bot_Core.Models {
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Util = Scripts.Util;

    internal struct AccessTokenData {
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

        public readonly string MaskedSerialized => JsonSerializer.Serialize(new {
            AccessToken = Util.GetMasked(AccessToken),
            ExpiresIn,
            RefreshToken = Util.GetMasked(RefreshToken),
            Scope,
            TokenType,
        });
    }
}
