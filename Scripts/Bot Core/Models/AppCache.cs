namespace Stonebot.Scripts.Bot_Core.Models {
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Util = Scripts.Util;

    internal struct AppCacheData {
        [JsonPropertyName("chatterRefreshToken")]
        public string ChatterRefreshToken { get; set; }
        [JsonPropertyName("collectorRefreshToken")]
        public string CollectorRefreshToken { get; set; }

        public readonly string MaskedSerialized => JsonSerializer.Serialize(new {
            ChatterRefreshToken = Util.GetMasked(ChatterRefreshToken),
            CollectorRefreshToken = Util.GetMasked(CollectorRefreshToken),
        });
    }
}
