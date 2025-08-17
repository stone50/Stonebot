namespace Stonebot.Models.Bodies {
    using System.Text.Json.Serialization;

    internal struct AddChannelChatMessageEventSubBody {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("condition")]
        public AddChannelChatMessageEventSubBodyCondition Condition { get; set; }
        [JsonPropertyName("transport")]
        public AddChannelChatMessageEventSubBodyTransport Transport { get; set; }
    }

    internal struct AddChannelChatMessageEventSubBodyCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }

    internal struct AddChannelChatMessageEventSubBodyTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}
