namespace Stonebot.Models.Bodies {
    using System.Text.Json.Serialization;

    internal class PostAddChannelChatMessageEventSubBody {
        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("version")]
        public required string Version { get; init; }

        [JsonPropertyName("condition")]
        public required PostAddChannelChatMessageEventSubBodyCondition Condition { get; init; }

        [JsonPropertyName("transport")]
        public required PostAddChannelChatMessageEventSubBodyTransport Transport { get; init; }
    }

    internal class PostAddChannelChatMessageEventSubBodyCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public required string BroadcasterId { get; init; }

        [JsonPropertyName("user_id")]
        public required string UserId { get; init; }
    }

    internal class PostAddChannelChatMessageEventSubBodyTransport {
        [JsonPropertyName("method")]
        public required string Method { get; init; }

        [JsonPropertyName("session_id")]
        public required string SessionId { get; init; }
    }
}
