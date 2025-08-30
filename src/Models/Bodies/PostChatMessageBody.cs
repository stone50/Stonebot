namespace Stonebot.Models.Bodies {
    using System.Text.Json.Serialization;

    internal struct PostChatMessageBody {
        [JsonPropertyName("broadcaster_id")]
        [JsonRequired]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("sender_id")]
        [JsonRequired]
        public string SenderId { get; set; }

        [JsonPropertyName("message")]
        [JsonRequired]
        public string Message { get; set; }

        [JsonPropertyName("reply_parent_message_id")]
        [JsonRequired]
        public string? ReplyParentMessageId { get; set; }
    }
}
