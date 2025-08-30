namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubKeepaliveMessage {
        [JsonPropertyName("metadata")]
        [JsonRequired]
        public EventSubKeepaliveMessageMetadata Metadata { get; set; }

        [JsonPropertyName("payload")]
        [JsonRequired]
        public EventSubKeepaliveMessagePayload Payload { get; set; }
    }

    internal struct EventSubKeepaliveMessageMetadata {
        [JsonPropertyName("message_id")]
        [JsonRequired]
        public string MessageId { get; set; }

        [JsonPropertyName("message_type")]
        [JsonRequired]
        public string MessageType { get; set; }

        [JsonPropertyName("message_timestamp")]
        [JsonRequired]
        public string MessageTimestamp { get; set; }
    }

    internal struct EventSubKeepaliveMessagePayload { }
}
