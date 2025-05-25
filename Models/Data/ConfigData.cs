namespace Stonebot.Models.Data {
    using System.Text.Json.Serialization;

    internal struct ConfigData {
        [JsonPropertyName("chatter_client_id")]
        public string ChatterClientId { get; set; }
        [JsonPropertyName("chatter_client_secret")]
        public string ChatterClientSecret { get; set; }
        [JsonPropertyName("collector_client_id")]
        public string BroadcasterClientId { get; set; }
        [JsonPropertyName("collector_client_secret")]
        public string BroadcasterClientSecret { get; set; }
        [JsonPropertyName("authorization_port")]
        public int AuthorizationPort { get; set; }

        [JsonPropertyName("num_max_log_files")]
        public int NumMaxLogFiles { get; set; }
    }
}
