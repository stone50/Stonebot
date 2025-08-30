namespace Stonebot.Models.Bodies {
    using System.Text.Json.Serialization;

    internal struct PostAddChannelChatMessageEventSubBody {
        [JsonPropertyName("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonPropertyName("version")]
        [JsonRequired]
        public string Version { get; set; }

        [JsonPropertyName("condition")]
        [JsonRequired]
        public PostAddChannelChatMessageEventSubBodyCondition Condition { get; set; }

        [JsonPropertyName("transport")]
        [JsonRequired]
        public PostAddChannelChatMessageEventSubBodyTransport Transport { get; set; }
    }

    internal struct PostAddChannelChatMessageEventSubBodyCondition {
        [JsonPropertyName("broadcaster_user_id")]
        [JsonRequired]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("user_id")]
        [JsonRequired]
        public string UserId { get; set; }
    }

    internal struct PostAddChannelChatMessageEventSubBodyTransport {
        [JsonPropertyName("method")]
        [JsonRequired]
        public string Method { get; set; }

        [JsonPropertyName("session_id")]
        [JsonRequired]
        public string SessionId { get; set; }
    }
}
