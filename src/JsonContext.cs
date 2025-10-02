namespace Stonebot {
    using Models.Bodies;
    using Models.EventSubMessages;
    using Models.Responses;
    using System.Text.Json.Serialization;

    [JsonSerializable(typeof(PostAddChannelChatMessageEventSubBody))]
    [JsonSerializable(typeof(PostChatMessageBody))]
    [JsonSerializable(typeof(EventSubKeepaliveMessage))]
    [JsonSerializable(typeof(EventSubNotificationMessage))]
    [JsonSerializable(typeof(EventSubReconnectMessage))]
    [JsonSerializable(typeof(EventSubRevocationMessage))]
    [JsonSerializable(typeof(EventSubWelcomeMessage))]
    [JsonSerializable(typeof(GetAccessTokenResponse))]
    [JsonSerializable(typeof(GetEventSubsResponse))]
    [JsonSerializable(typeof(GetUsersResponse))]
    [JsonSerializable(typeof(PostChatMessageResponse))]
    [JsonSerializable(typeof(PostDeviceCodeResponse))]
    internal partial class JsonContext : JsonSerializerContext { }
}
