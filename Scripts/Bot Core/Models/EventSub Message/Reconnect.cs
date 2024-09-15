namespace StoneBot.Scripts.Bot_Core.Models.EventSub_Message {
    using System.Text.Json.Serialization;

    public struct EventSubReconnectMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
    }

    public struct EventSubReconnectMessagePayload {
        [JsonPropertyName("session")]
        public EventSubSessionData Session { get; set; }
    }

    public struct EventSubReconnectMessage {
        [JsonPropertyName("metadata")]
        public EventSubReconnectMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubReconnectMessagePayload Payload { get; set; }
    }
}
