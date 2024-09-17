namespace StoneBot.Scripts.Bot_Core.Models.EventSub_Message {
    using EventSub;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal struct EventSubNotificationMessageMetadata {
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

    internal struct EventSubNotificationMessagePayload {
        [JsonPropertyName("subscription")]
        public EventSubData Subscription { get; set; }
        [JsonPropertyName("event")]
        public JsonElement Event { get; set; }
    }

    internal struct EventSubNotificationMessage {
        [JsonPropertyName("metadata")]
        public EventSubNotificationMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubNotificationMessagePayload Payload { get; set; }
    }
}
