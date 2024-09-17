namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct AppCacheData {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
