namespace Stonebot {
    using Models;
    using System.Text.Json.Serialization;

    [JsonSerializable(typeof(AccessTokenData))]
    [JsonSerializable(typeof(AddEventSubContent))]
    [JsonSerializable(typeof(CacheData))]
    [JsonSerializable(typeof(ConfigData))]
    [JsonSerializable(typeof(EventSubKeepaliveMessage))]
    [JsonSerializable(typeof(EventSubNotificationMessage))]
    [JsonSerializable(typeof(EventSubReconnectMessage))]
    [JsonSerializable(typeof(EventSubRevocationMessage))]
    [JsonSerializable(typeof(EventSubsData))]
    [JsonSerializable(typeof(EventSubWelcomeMessage))]
    [JsonSerializable(typeof(PaginatedSubscriptionsData))]
    [JsonSerializable(typeof(UsersData))]
    internal partial class JsonContext : JsonSerializerContext { }
}
