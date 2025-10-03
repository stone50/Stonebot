namespace Stonebot.Models.Bodies {
    using System.Text.Json.Serialization;

    internal class PostChatMessageBody {
        [JsonPropertyName("broadcaster_id")]
        public required string BroadcasterId { get; init; }

        [JsonPropertyName("sender_id")]
        public required string SenderId { get; init; }

        [JsonPropertyName("message")]
        public required string Message { get; init; }

        [JsonPropertyName("reply_parent_message_id")]
        public string? ReplyParentMessageId { get; init; }
    }
}
