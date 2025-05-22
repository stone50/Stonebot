namespace Stonebot.Models {
    using System.Text.Json.Serialization;

    internal struct EventSubs {
        [JsonPropertyName("data")]
        public EventSubsDataPoint[] Data { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("total_cost")]
        public int TotalCost { get; set; }
        [JsonPropertyName("max_total_cost")]
        public int MaxTotalCost { get; set; }
        [JsonPropertyName("pagination")]
        public EventSubsPagination Pagination { get; set; }
    }

    internal struct EventSubsDataPoint {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("condition")]
        public ChannelChatMessageEventSubCondition Condition { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("transport")]
        public EventSubsDataPointTransport Transport { get; set; }
        [JsonPropertyName("cost")]
        public int Cost { get; set; }
    }

    internal struct EventSubsDataPointTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
        [JsonPropertyName("connected_at")]
        public string ConnectedAt { get; set; }
        [JsonPropertyName("disconnected_at")]
        public string DisconnectedAt { get; set; }
    }

    internal struct EventSubsPagination {
        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }
}
