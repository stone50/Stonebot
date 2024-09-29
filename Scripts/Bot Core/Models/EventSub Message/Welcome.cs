namespace Stonebot.Scripts.Bot_Core.Models.EventSub_Message {
    using System.Text.Json.Serialization;

    internal struct EventSubWelcomeMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
    }

    internal struct EventSubWelcomeMessagePayload {
        [JsonPropertyName("session")]
        public EventSubSessionData Session { get; set; }
    }

    internal struct EventSubWelcomeMessage {
        [JsonPropertyName("metadata")]
        public EventSubWelcomeMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubWelcomeMessagePayload Payload { get; set; }
    }
}
