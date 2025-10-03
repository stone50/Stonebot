namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal class EventSubKeepaliveMessage {
        [JsonPropertyName("metadata")]
        public required EventSubKeepaliveMessageMetadata Metadata { get; init; }

        [JsonPropertyName("payload")]
        public required EventSubKeepaliveMessagePayload Payload { get; init; }
    }

    internal class EventSubKeepaliveMessageMetadata {
        [JsonPropertyName("message_id")]
        public required string MessageId { get; init; }

        [JsonPropertyName("message_type")]
        public required string MessageType { get; init; }

        [JsonPropertyName("message_timestamp")]
        public required string MessageTimestamp { get; init; }
    }

    internal class EventSubKeepaliveMessagePayload { }
}
