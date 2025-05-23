namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubRevocationMessage {
        [JsonPropertyName("metadata")]
        public EventSubRevocationMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubRevocationMessagePayload Payload { get; set; }
    }

    internal struct EventSubRevocationMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
        [JsonPropertyName("subscription_type")]
        public string SubscriptionType { get; set; }
        [JsonPropertyName("subscription_version")]
        public string SubscriptionVersion { get; set; }
    }

    internal struct EventSubRevocationMessagePayload {
        [JsonPropertyName("subscription")]
        public EventSubRevocationMessagePayloadSubscription Subscription { get; set; }
    }

    internal struct EventSubRevocationMessagePayloadSubscription {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("cost")]
        public int Cost { get; set; }
        [JsonPropertyName("condition")]
        public EventSubRevocationMessagePayloadSubscriptionCondition Condition { get; set; }
        [JsonPropertyName("transport")]
        public EventSubRevocationMessagePayloadSubscriptionTransport Transport { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }

    internal struct EventSubRevocationMessagePayloadSubscriptionCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }

    internal struct EventSubRevocationMessagePayloadSubscriptionTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}
