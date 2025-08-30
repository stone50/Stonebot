namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubReconnectMessage {
        [JsonPropertyName("metadata")]
        [JsonRequired]
        public EventSubReconnectMessageMetadata Metadata { get; set; }

        [JsonPropertyName("payload")]
        [JsonRequired]
        public EventSubReconnectMessagePayload Payload { get; set; }
    }

    internal struct EventSubReconnectMessageMetadata {
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

    internal struct EventSubReconnectMessagePayload {
        [JsonPropertyName("session")]
        [JsonRequired]
        public EventSubReconnectMessagePayloadSession Session { get; set; }
    }

    internal struct EventSubReconnectMessagePayloadSession {
        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        [JsonRequired]
        public string Status { get; set; }

        [JsonPropertyName("keepalive_timeout_seconds")]
        public int? KeepaliveTimeoutSeconds { get; set; }

        [JsonPropertyName("reconnect_url")]
        [JsonRequired]
        public string ReconnectUrl { get; set; }

        [JsonPropertyName("connected_at")]
        [JsonRequired]
        public string ConnectedAt { get; set; }
    }
}
