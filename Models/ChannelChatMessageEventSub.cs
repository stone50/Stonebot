namespace Stonebot.Models {
    using System.Text.Json.Serialization;

    internal struct ChannelChatMessageEventSubCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }

    internal struct AddChannelChatMessageEventSub {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("condition")]
        public ChannelChatMessageEventSubCondition Condition { get; set; }
        [JsonPropertyName("transport")]
        public AddChannelChatMessageEventSubTransport Transport { get; set; }
    }

    internal struct AddChannelChatMessageEventSubTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}
