namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    public class PostChatMessageResponse {
        [JsonPropertyName("data")]
        public required SendChatMessageResponseDataPoint[] Data { get; init; }

        [JsonPropertyName("drop_reason")]
        public SendChatMessageResponseDropReason? DropReason { get; init; }
    }

    public class SendChatMessageResponseDataPoint {
        [JsonPropertyName("message_id")]
        public required string MessageId { get; init; }

        [JsonPropertyName("is_sent")]
        public required bool IsSent { get; init; }
    }

    public class SendChatMessageResponseDropReason {
        [JsonPropertyName("code")]
        public required string Code { get; init; }

        [JsonPropertyName("message")]
        public required string Message { get; init; }
    }
}
