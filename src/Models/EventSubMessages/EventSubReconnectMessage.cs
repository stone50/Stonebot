namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal class EventSubReconnectMessage {
        [JsonPropertyName("metadata")]
        public required EventSubReconnectMessageMetadata Metadata { get; init; }

        [JsonPropertyName("payload")]
        public required EventSubReconnectMessagePayload Payload { get; init; }
    }

    internal class EventSubReconnectMessageMetadata {
        [JsonPropertyName("message_id")]
        public required string MessageId { get; init; }

        [JsonPropertyName("message_type")]
        public required string MessageType { get; init; }

        [JsonPropertyName("message_timestamp")]
        public required string MessageTimestamp { get; init; }
    }

    internal class EventSubReconnectMessagePayload {
        [JsonPropertyName("session")]
        public required EventSubReconnectMessagePayloadSession Session { get; init; }
    }

    internal class EventSubReconnectMessagePayloadSession {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("keepalive_timeout_seconds")]
        public int? KeepaliveTimeoutSeconds { get; init; }

        [JsonPropertyName("reconnect_url")]
        public required string ReconnectUrl { get; init; }

        [JsonPropertyName("connected_at")]
        public required string ConnectedAt { get; init; }
    }
}
