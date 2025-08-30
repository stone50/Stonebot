namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubRevocationMessage {
        [JsonPropertyName("metadata")]
        [JsonRequired]
        public EventSubRevocationMessageMetadata Metadata { get; set; }

        [JsonPropertyName("payload")]
        [JsonRequired]
        public EventSubRevocationMessagePayload Payload { get; set; }
    }

    internal struct EventSubRevocationMessageMetadata {
        [JsonPropertyName("message_id")]
        [JsonRequired]
        public string MessageId { get; set; }

        [JsonPropertyName("message_type")]
        [JsonRequired]
        public string MessageType { get; set; }

        [JsonPropertyName("message_timestamp")]
        [JsonRequired]
        public string MessageTimestamp { get; set; }

        [JsonPropertyName("subscription_type")]
        [JsonRequired]
        public string SubscriptionType { get; set; }

        [JsonPropertyName("subscription_version")]
        [JsonRequired]
        public string SubscriptionVersion { get; set; }
    }

    internal struct EventSubRevocationMessagePayload {
        [JsonPropertyName("subscription")]
        [JsonRequired]
        public EventSubRevocationMessagePayloadSubscription Subscription { get; set; }
    }

    internal struct EventSubRevocationMessagePayloadSubscription {
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

        [JsonPropertyName("cost")]
        [JsonRequired]
        public int Cost { get; set; }

        [JsonPropertyName("condition")]
        [JsonRequired]
        public EventSubRevocationMessagePayloadSubscriptionCondition Condition { get; set; }

        [JsonPropertyName("transport")]
        [JsonRequired]
        public EventSubRevocationMessagePayloadSubscriptionTransport Transport { get; set; }

        [JsonPropertyName("created_at")]
        [JsonRequired]
        public string CreatedAt { get; set; }
    }

    internal struct EventSubRevocationMessagePayloadSubscriptionCondition {
        [JsonPropertyName("broadcaster_user_id")]
        [JsonRequired]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("user_id")]
        [JsonRequired]
        public string UserId { get; set; }
    }

    internal struct EventSubRevocationMessagePayloadSubscriptionTransport {
        [JsonPropertyName("method")]
        [JsonRequired]
        public string Method { get; set; }

        [JsonPropertyName("session_id")]
        [JsonRequired]
        public string SessionId { get; set; }
    }
}
