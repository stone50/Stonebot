namespace Stonebot.Scripts.Bot_Core.Models.EventSub_Message {
    using EventSub;
    using System.Text.Json.Serialization;

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
        public EventSubData Subscription { get; set; }
    }

    internal struct EventSubRevocationMessage {
        [JsonPropertyName("metadata")]
        public EventSubRevocationMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubRevocationMessagePayload Payload { get; set; }
    }
}
