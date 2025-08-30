namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubWelcomeMessage {
        [JsonPropertyName("metadata")]
        [JsonRequired]
        public EventSubWelcomeMessageMetadata Metadata { get; set; }

        [JsonPropertyName("payload")]
        [JsonRequired]
        public EventSubWelcomeMessagePayload Payload { get; set; }
    }

    internal struct EventSubWelcomeMessageMetadata {
        [JsonPropertyName("message_id")]
        [JsonRequired]
        public string MessageId { get; set; }

        [JsonPropertyName("message_type")]
        [JsonRequired]
        public string MessageType { get; set; }

        [JsonPropertyName("message_timestamp")]
        [JsonRequired]
        public string MessageTimestamp { get; set; }
    }

    internal struct EventSubWelcomeMessagePayload {
        [JsonPropertyName("session")]
        [JsonRequired]
        public EventSubWelcomeMessagePayloadSession Session { get; set; }
    }

    internal struct EventSubWelcomeMessagePayloadSession {
        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        [JsonRequired]
        public string Status { get; set; }

        [JsonPropertyName("keepalive_timeout_seconds")]
        [JsonRequired]
        public int KeepaliveTimeoutSeconds { get; set; }

        [JsonPropertyName("reconnect_url")]
        public string? ReconnectUrl { get; set; }

        [JsonPropertyName("connected_at")]
        [JsonRequired]
        public string ConnectedAt { get; set; }
    }
}
