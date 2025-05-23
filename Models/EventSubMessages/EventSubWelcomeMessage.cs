namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubWelcomeMessage {
        [JsonPropertyName("metadata")]
        public EventSubWelcomeMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubWelcomeMessagePayload Payload { get; set; }
    }

    internal struct EventSubWelcomeMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
    }

    internal struct EventSubWelcomeMessagePayload {
        [JsonPropertyName("session")]
        public EventSubWelcomeMessagePayloadSession Session { get; set; }
    }

    internal struct EventSubWelcomeMessagePayloadSession {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("keepalive_timeout_seconds")]
        public int KeepaliveTimeoutSeconds { get; set; }
        [JsonPropertyName("reconnect_url")]
        public string? ReconnectUrl { get; set; }
        [JsonPropertyName("connected_at")]
        public string ConnectedAt { get; set; }
    }
}
