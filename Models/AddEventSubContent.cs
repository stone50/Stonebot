namespace Stonebot.Models {
    using System.Text.Json.Serialization;

    internal struct AddEventSubTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }

    internal struct AddEventSubContent {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("condition")]
        public object Condition { get; set; }
        [JsonPropertyName("transport")]
        public AddEventSubTransport Transport { get; set; }
    }
}
