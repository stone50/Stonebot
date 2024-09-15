namespace StoneBot.Scripts.Bot_Core.Models.EventSub_Message {
    using System.Text.Json.Serialization;

    public struct EventSubKeepaliveMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
    }

    public struct EventSubKeepaliveMessagePayload { }

    public struct EventSubKeepaliveMessage {
        [JsonPropertyName("metadata")]
        public EventSubKeepaliveMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubKeepaliveMessagePayload Payload { get; set; }
    }
}
