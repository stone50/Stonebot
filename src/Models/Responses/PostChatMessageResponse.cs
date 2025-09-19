namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    public struct PostChatMessageResponse {
        [JsonPropertyName("data")]
        [JsonRequired]
        public SendChatMessageResponseDataPoint[] Data { get; set; }

        [JsonPropertyName("drop_reason")]
        public SendChatMessageResponseDropReason? DropReason { get; set; }
    }

    public struct SendChatMessageResponseDataPoint {
        [JsonPropertyName("message_id")]
        [JsonRequired]
        public string MessageId { get; set; }

        [JsonPropertyName("is_sent")]
        [JsonRequired]
        public bool IsSent { get; set; }
    }

    public struct SendChatMessageResponseDropReason {
        [JsonPropertyName("code")]
        [JsonRequired]
        public string Code { get; set; }

        [JsonPropertyName("message")]
        [JsonRequired]
        public string Message { get; set; }
    }
}
