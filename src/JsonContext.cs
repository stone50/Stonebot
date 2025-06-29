namespace Stonebot {
    using Models;
    using Models.Data;
    using Models.EventSubMessages;
    using Models.Responses;
    using System.Collections.Concurrent;
    using System.Text.Json.Serialization;

    [JsonSerializable(typeof(AccessTokenData))]
    [JsonSerializable(typeof(AddChannelChatMessageEventSub))]
    [JsonSerializable(typeof(CacheData))]
    [JsonSerializable(typeof(ConfigData))]
    [JsonSerializable(typeof(EventSubKeepaliveMessage))]
    [JsonSerializable(typeof(EventSubNotificationMessage))]
    [JsonSerializable(typeof(EventSubReconnectMessage))]
    [JsonSerializable(typeof(EventSubRevocationMessage))]
    [JsonSerializable(typeof(EventSubWelcomeMessage))]
    [JsonSerializable(typeof(GetEventSubsResponse))]
    [JsonSerializable(typeof(GetModeratorsResponse))]
    [JsonSerializable(typeof(GetSubscriptionResponse))]
    [JsonSerializable(typeof(GetUsersResponse))]
    [JsonSerializable(typeof(GetVIPsResponse))]
    [JsonSerializable(typeof(SendChatMessage))]
    [JsonSerializable(typeof(SendChatMessageResponse))]
    [JsonSerializable(typeof(CommandManagerData))]
    [JsonSerializable(typeof(ConcurrentDictionary<string, object>))]
    internal partial class JsonContext : JsonSerializerContext { }
}
