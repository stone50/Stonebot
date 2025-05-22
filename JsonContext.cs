namespace Stonebot {
    using Models;
    using System.Text.Json.Serialization;

    [JsonSerializable(typeof(AccessTokenData))]
    [JsonSerializable(typeof(AddChannelChatMessageEventSub))]
    [JsonSerializable(typeof(CacheData))]
    [JsonSerializable(typeof(ConfigData))]
    [JsonSerializable(typeof(EventSubKeepaliveMessage))]
    [JsonSerializable(typeof(EventSubNotificationMessage))]
    [JsonSerializable(typeof(EventSubReconnectMessage))]
    [JsonSerializable(typeof(EventSubRevocationMessage))]
    [JsonSerializable(typeof(EventSubs))]
    [JsonSerializable(typeof(EventSubWelcomeMessage))]
    [JsonSerializable(typeof(UsersData))]
    internal partial class JsonContext : JsonSerializerContext { }
}
