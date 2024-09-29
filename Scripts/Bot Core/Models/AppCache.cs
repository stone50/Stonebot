namespace Stonebot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct AppCacheData {
        [JsonPropertyName("chatterRefreshToken")]
        public string ChatterRefreshToken { get; set; }
        [JsonPropertyName("collectorRefreshToken")]
        public string CollectorRefreshToken { get; set; }
    }
}
