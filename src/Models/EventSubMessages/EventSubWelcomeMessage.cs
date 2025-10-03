namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal class EventSubWelcomeMessage {
        [JsonPropertyName("metadata")]
        public required EventSubWelcomeMessageMetadata Metadata { get; init; }

        [JsonPropertyName("payload")]
        public required EventSubWelcomeMessagePayload Payload { get; init; }
    }

    internal class EventSubWelcomeMessageMetadata {
        [JsonPropertyName("message_id")]
        public required string MessageId { get; init; }

        [JsonPropertyName("message_type")]
        public required string MessageType { get; init; }

        [JsonPropertyName("message_timestamp")]
        public required string MessageTimestamp { get; init; }
    }

    internal class EventSubWelcomeMessagePayload {
        [JsonPropertyName("session")]
        public required EventSubWelcomeMessagePayloadSession Session { get; init; }
    }

    internal class EventSubWelcomeMessagePayloadSession {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("keepalive_timeout_seconds")]
        public required int KeepaliveTimeoutSeconds { get; init; }

        [JsonPropertyName("reconnect_url")]
        public string? ReconnectUrl { get; init; }

        [JsonPropertyName("connected_at")]
        public required string ConnectedAt { get; init; }
    }
}
