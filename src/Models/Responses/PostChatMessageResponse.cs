namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    public struct PostChatMessageResponse {
        [JsonPropertyName("data")]
        public SendChatMessageResponseDataPoint[] Data { get; set; }
        [JsonPropertyName("drop_reason")]
        public SendChatMessageResponseDropReason DropReason { get; set; }
    }

    public struct SendChatMessageResponseDataPoint {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("is_sent")]
        public bool IsSent { get; set; }
    }

    public struct SendChatMessageResponseDropReason {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
