namespace Stonebot.Models.Bodies {
    using System.Text.Json.Serialization;

    internal struct SendChatMessageBody {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("sender_id")]
        public string SenderId { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("reply_parent_message_id")]
        public string? ReplyParentMessageId { get; set; }
    }
}
