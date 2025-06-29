namespace Stonebot.Models {
    using System.Text.Json.Serialization;

    internal struct SendChatMessage {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("sender_id")]
        public string SenderId { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("reply_parent_message_id")]
        public string? ReplyParentMessageId { get; set; }
    }

    public struct SendChatMessageResponse {
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
