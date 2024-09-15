namespace StoneBot.Scripts.Models.EventSub_Message {
    using System.Text.Json.Serialization;

    public struct EventSubWelcomeMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
    }

    public struct EventSubWelcomeMessagePayload {
        [JsonPropertyName("session")]
        public EventSubSessionData Session { get; set; }
    }

    public struct EventSubWelcomeMessage {
        [JsonPropertyName("metadata")]
        public EventSubWelcomeMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubWelcomeMessagePayload Payload { get; set; }
    }
}
