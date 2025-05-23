namespace Stonebot.Models.Data {
    using System.Text.Json.Serialization;

    internal struct CacheData {
        [JsonPropertyName("chatter_refresh_token")]
        public string? ChatterRefreshToken { get; set; }
        [JsonPropertyName("collector_refresh_token")]
        public string? BroadcasterRefreshToken { get; set; }
    }
}
