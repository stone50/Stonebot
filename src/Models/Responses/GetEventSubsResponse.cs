namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal class GetEventSubsResponse {
        [JsonPropertyName("data")]
        public required GetEventSubsResponseDataPoint[] Data { get; init; }

        [JsonPropertyName("total")]
        public required int Total { get; init; }

        [JsonPropertyName("total_cost")]
        public required int TotalCost { get; init; }

        [JsonPropertyName("max_total_cost")]
        public required int MaxTotalCost { get; init; }

        [JsonPropertyName("pagination")]
        public required GetEventSubsResponsePagination Pagination { get; init; }
    }

    internal class GetEventSubsResponseDataPoint {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("version")]
        public required string Version { get; init; }

        [JsonPropertyName("condition")]
        public required GetEventSubsResponseDataPointCondition Condition { get; init; }

        [JsonPropertyName("created_at")]
        public required string CreatedAt { get; init; }

        [JsonPropertyName("transport")]
        public required GetEventSubsResponseDataPointTransport Transport { get; init; }

        [JsonPropertyName("cost")]
        public required int Cost { get; init; }
    }

    internal class GetEventSubsResponseDataPointCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public required string BroadcasterId { get; init; }

        [JsonPropertyName("user_id")]
        public required string UserId { get; init; }
    }

    internal class GetEventSubsResponseDataPointTransport {
        [JsonPropertyName("method")]
        public required string Method { get; init; }

        [JsonPropertyName("session_id")]
        public required string SessionId { get; init; }

        [JsonPropertyName("connected_at")]
        public required string ConnectedAt { get; init; }

        [JsonPropertyName("disconnected_at")]
        public string? DisconnectedAt { get; init; }
    }

    internal class GetEventSubsResponsePagination {
        [JsonPropertyName("cursor")]
        public string? Cursor { get; init; }
    }
}
