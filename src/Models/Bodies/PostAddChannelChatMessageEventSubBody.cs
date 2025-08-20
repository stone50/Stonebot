namespace Stonebot.Models.Bodies {
    using System.Text.Json.Serialization;

    internal struct PostAddChannelChatMessageEventSubBody {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("condition")]
        public PostAddChannelChatMessageEventSubBodyCondition Condition { get; set; }
        [JsonPropertyName("transport")]
        public PostAddChannelChatMessageEventSubBodyTransport Transport { get; set; }
    }

    internal struct PostAddChannelChatMessageEventSubBodyCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }

    internal struct PostAddChannelChatMessageEventSubBodyTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}
