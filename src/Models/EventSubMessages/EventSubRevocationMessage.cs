namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal class EventSubRevocationMessage {
        [JsonPropertyName("metadata")]
        public required EventSubRevocationMessageMetadata Metadata { get; init; }

        [JsonPropertyName("payload")]
        public required EventSubRevocationMessagePayload Payload { get; init; }
    }

    internal class EventSubRevocationMessageMetadata {
        [JsonPropertyName("message_id")]
        public required string MessageId { get; init; }

        [JsonPropertyName("message_type")]
        public required string MessageType { get; init; }

        [JsonPropertyName("message_timestamp")]
        public required string MessageTimestamp { get; init; }

        [JsonPropertyName("subscription_type")]
        public required string SubscriptionType { get; init; }

        [JsonPropertyName("subscription_version")]
        public required string SubscriptionVersion { get; init; }
    }

    internal class EventSubRevocationMessagePayload {
        [JsonPropertyName("subscription")]
        public required EventSubRevocationMessagePayloadSubscription Subscription { get; init; }
    }

    internal class EventSubRevocationMessagePayloadSubscription {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("version")]
        public required string Version { get; init; }

        [JsonPropertyName("cost")]
        public required int Cost { get; init; }

        [JsonPropertyName("condition")]
        public required EventSubRevocationMessagePayloadSubscriptionCondition Condition { get; init; }

        [JsonPropertyName("transport")]
        public required EventSubRevocationMessagePayloadSubscriptionTransport Transport { get; init; }

        [JsonPropertyName("created_at")]
        public required string CreatedAt { get; init; }
    }

    internal class EventSubRevocationMessagePayloadSubscriptionCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public required string BroadcasterId { get; init; }

        [JsonPropertyName("user_id")]
        public required string UserId { get; init; }
    }

    internal class EventSubRevocationMessagePayloadSubscriptionTransport {
        [JsonPropertyName("method")]
        public required string Method { get; init; }

        [JsonPropertyName("session_id")]
        public required string SessionId { get; init; }
    }
}
