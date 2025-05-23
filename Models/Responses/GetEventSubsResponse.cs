namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal struct GetEventSubsResponse {
        [JsonPropertyName("data")]
        public GetEventSubsResponseDataPoint[] Data { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("total_cost")]
        public int TotalCost { get; set; }
        [JsonPropertyName("max_total_cost")]
        public int MaxTotalCost { get; set; }
        [JsonPropertyName("pagination")]
        public GetEventSubsResponsePagination Pagination { get; set; }
    }

    internal struct GetEventSubsResponseDataPoint {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("condition")]
        public GetEventSubsResponseDataPointCondition Condition { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("transport")]
        public GetEventSubsResponseDataPointTransport Transport { get; set; }
        [JsonPropertyName("cost")]
        public int Cost { get; set; }
    }

    internal struct GetEventSubsResponseDataPointCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }

    internal struct GetEventSubsResponseDataPointTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
        [JsonPropertyName("connected_at")]
        public string ConnectedAt { get; set; }
        [JsonPropertyName("disconnected_at")]
        public string DisconnectedAt { get; set; }
    }

    internal struct GetEventSubsResponsePagination {
        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }
}
