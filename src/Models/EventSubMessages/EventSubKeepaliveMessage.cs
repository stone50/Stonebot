namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubKeepaliveMessage {
        [JsonPropertyName("metadata")]
        public EventSubKeepaliveMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubKeepaliveMessagePayload Payload { get; set; }
    }

    internal struct EventSubKeepaliveMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
    }

    internal struct EventSubKeepaliveMessagePayload { }
}
