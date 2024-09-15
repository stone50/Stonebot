namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public struct TransportData {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
        [JsonPropertyName("connected_at")]
        public string? ConnectedAt { get; set; }
        [JsonPropertyName("disconnected_at")]
        public string? DisconnectedAt { get; set; }
    }

    public struct EventSubData {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("condition")]
        public JsonElement Condition { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("transport")]
        public TransportData Transport { get; set; }
        [JsonPropertyName("cost")]
        public int Cost { get; set; }
    }

    public struct PaginationData {
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }

    public struct EventSubsData {
        [JsonPropertyName("data")]
        public EventSubData[] Data { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("total_cost")]
        public int TotalCost { get; set; }
        [JsonPropertyName("Max_total_cost")]
        public int MaxTotalCost { get; set; }
        [JsonPropertyName("pagination")]
        public PaginationData Pagination { get; set; }
    }
}
