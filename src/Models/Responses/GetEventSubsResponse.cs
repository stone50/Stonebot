namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal struct GetEventSubsResponse {
        [JsonPropertyName("data")]
        [JsonRequired]
        public GetEventSubsResponseDataPoint[] Data { get; set; }

        [JsonPropertyName("total")]
        [JsonRequired]
        public int Total { get; set; }

        [JsonPropertyName("total_cost")]
        [JsonRequired]
        public int TotalCost { get; set; }

        [JsonPropertyName("max_total_cost")]
        [JsonRequired]
        public int MaxTotalCost { get; set; }

        [JsonPropertyName("pagination")]
        [JsonRequired]
        public GetEventSubsResponsePagination Pagination { get; set; }
    }

    internal struct GetEventSubsResponseDataPoint {
        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        [JsonRequired]
        public string Status { get; set; }

        [JsonPropertyName("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonPropertyName("version")]
        [JsonRequired]
        public string Version { get; set; }

        [JsonPropertyName("condition")]
        [JsonRequired]
        public GetEventSubsResponseDataPointCondition Condition { get; set; }

        [JsonPropertyName("created_at")]
        [JsonRequired]
        public string CreatedAt { get; set; }

        [JsonPropertyName("transport")]
        [JsonRequired]
        public GetEventSubsResponseDataPointTransport Transport { get; set; }

        [JsonPropertyName("cost")]
        [JsonRequired]
        public int Cost { get; set; }
    }

    internal struct GetEventSubsResponseDataPointCondition {
        [JsonPropertyName("broadcaster_user_id")]
        [JsonRequired]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("user_id")]
        [JsonRequired]
        public string UserId { get; set; }
    }

    internal struct GetEventSubsResponseDataPointTransport {
        [JsonPropertyName("method")]
        [JsonRequired]
        public string Method { get; set; }

        [JsonPropertyName("session_id")]
        [JsonRequired]
        public string SessionId { get; set; }

        [JsonPropertyName("connected_at")]
        [JsonRequired]
        public string ConnectedAt { get; set; }

        [JsonPropertyName("disconnected_at")]
        [JsonRequired]
        public string DisconnectedAt { get; set; }
    }

    internal struct GetEventSubsResponsePagination {
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }
}
