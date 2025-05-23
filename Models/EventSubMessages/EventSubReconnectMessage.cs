namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubReconnectMessage {
        [JsonPropertyName("metadata")]
        public EventSubReconnectMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubReconnectMessagePayload Payload { get; set; }
    }

    internal struct EventSubReconnectMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
    }

    internal struct EventSubReconnectMessagePayload {
        [JsonPropertyName("session")]
        public EventSubReconnectMessagePayloadSession Session { get; set; }
    }

    internal struct EventSubReconnectMessagePayloadSession {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("keepalive_timeout_seconds")]
        public int? KeepaliveTimeoutSeconds { get; set; }
        [JsonPropertyName("reconnect_url")]
        public string ReconnectUrl { get; set; }
        [JsonPropertyName("connected_at")]
        public string ConnectedAt { get; set; }
    }
}
