namespace StoneBot.Scripts.Bot_Core.Models.EventSub_Message {
    using System.Text.Json.Serialization;

    public struct EventSubRevocationMessageMetadata {
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

    public struct EventSubRevocationMessagePayload {
        [JsonPropertyName("subscription")]
        public EventSubData Subscription { get; set; }
    }

    public struct EventSubRevocationMessage {
        [JsonPropertyName("metadata")]
        public EventSubRevocationMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubRevocationMessagePayload Payload { get; set; }
    }
}
