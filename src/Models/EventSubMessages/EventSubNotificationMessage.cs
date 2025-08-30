namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal struct EventSubNotificationMessage {
        [JsonPropertyName("metadata")]
        [JsonRequired]
        public EventSubNotificationMessageMetadata Metadata { get; set; }

        [JsonPropertyName("payload")]
        [JsonRequired]
        public EventSubNotificationMessagePayload Payload { get; set; }
    }

    internal struct EventSubNotificationMessageMetadata {
        [JsonPropertyName("message_id")]
        [JsonRequired]
        public string MessageId { get; set; }

        [JsonPropertyName("message_type")]
        [JsonRequired]
        public string MessageType { get; set; }

        [JsonPropertyName("message_timestamp")]
        [JsonRequired]
        public string MessageTimestamp { get; set; }

        [JsonPropertyName("subscription_type")]
        [JsonRequired]
        public string SubscriptionType { get; set; }

        [JsonPropertyName("subscription_version")]
        [JsonRequired]
        public string SubscriptionVersion { get; set; }
    }

    internal struct EventSubNotificationMessagePayload {
        [JsonPropertyName("subscription")]
        [JsonRequired]
        public EventSubNotificationMessagePayloadSubscription Subscription { get; set; }

        [JsonPropertyName("event")]
        [JsonRequired]
        public EventSubNotificationMessagePayloadEvent Event { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadSubscription {
        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        [JsonRequired]
        public string Status { get; set; }

        [JsonPropertyName("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonPropertyName("version")]
        [JsonRequired]
        public string Version { get; set; }

        [JsonPropertyName("cost")]
        [JsonRequired]
        public int Cost { get; set; }

        [JsonPropertyName("condition")]
        [JsonRequired]
        public EventSubNotificationMessagePayloadSubscriptionCondition Condition { get; set; }

        [JsonPropertyName("transport")]
        [JsonRequired]
        public EventSubNotificationMessagePayloadSubscriptionTransport Transport { get; set; }

        [JsonPropertyName("created_at")]
        [JsonRequired]
        public string CreatedAt { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadSubscriptionCondition {
        [JsonPropertyName("broadcaster_user_id")]
        [JsonRequired]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("user_id")]
        [JsonRequired]
        public string UserId { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadSubscriptionTransport {
        [JsonPropertyName("method")]
        [JsonRequired]
        public string Method { get; set; }

        [JsonPropertyName("session_id")]
        [JsonRequired]
        public string SessionId { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEvent {
        [JsonPropertyName("broadcaster_user_id")]
        [JsonRequired]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("broadcaster_user_name")]
        [JsonRequired]
        public string BroadcasterUserName { get; set; }

        [JsonPropertyName("broadcaster_user_login")]
        [JsonRequired]
        public string BroadcasterLogin { get; set; }

        [JsonPropertyName("chatter_user_id")]
        [JsonRequired]
        public string ChatterId { get; set; }

        [JsonPropertyName("chatter_user_name")]
        [JsonRequired]
        public string ChatterUserName { get; set; }

        [JsonPropertyName("chatter_user_login")]
        [JsonRequired]
        public string ChatterLogin { get; set; }

        [JsonPropertyName("message_id")]
        [JsonRequired]
        public string MessageId { get; set; }

        [JsonPropertyName("message")]
        [JsonRequired]
        public EventSubNotificationMessagePayloadEventMessage Message { get; set; }

        [JsonPropertyName("message_type")]
        [JsonRequired]
        public string MessageType { get; set; }

        [JsonPropertyName("badges")]
        [JsonRequired]
        public EventSubNotificationMessagePayloadEventBadge[] Badges { get; set; }

        [JsonPropertyName("cheer")]
        public EventSubNotificationMessagePayloadEventCheer? Cheer { get; set; }

        [JsonPropertyName("color")]
        [JsonRequired]
        public string Color { get; set; }

        [JsonPropertyName("reply")]
        public EventSubNotificationMessagePayloadEventReply? Reply { get; set; }

        [JsonPropertyName("channel_points_custom_reward_id")]
        public string? ChannelPointsCustomRewardId { get; set; }

        [JsonPropertyName("source_broadcaster_user_id")]
        public string? SourceBroadcasterId { get; set; }

        [JsonPropertyName("source_broadcaster_user_name")]
        public string? SourceBroadcasterUserName { get; set; }

        [JsonPropertyName("source_broadcaster_user_login")]
        public string? SourceBroadcasterLogin { get; set; }

        [JsonPropertyName("source_message_id")]
        public string? SourceMessageId { get; set; }

        [JsonPropertyName("source_badges")]
        public EventSubNotificationMessagePayloadEventSourceBadges? SourceBadges { get; set; }

        [JsonPropertyName("is_source_only")]
        public bool? IsSourceOnly { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventMessage {
        [JsonPropertyName("text")]
        [JsonRequired]
        public string Text { get; set; }

        [JsonPropertyName("fragments")]
        [JsonRequired]
        public EventSubNotificationMessagePayloadEventMessageFragment[] Fragments { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventMessageFragment {
        [JsonPropertyName("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        [JsonRequired]
        public string Text { get; set; }

        [JsonPropertyName("cheermote")]
        public EventSubNotificationMessagePayloadEventMessageFragmentCheermote? Cheermote { get; set; }

        [JsonPropertyName("emote")]
        public EventSubNotificationMessagePayloadEventMessageFragmentEmote? Emote { get; set; }

        [JsonPropertyName("mention")]
        public EventSubNotificationMessagePayloadEventMessageFragmentMention? Mention { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventMessageFragmentCheermote {
        [JsonPropertyName("prefix")]
        [JsonRequired]
        public string Prefix { get; set; }

        [JsonPropertyName("bits")]
        [JsonRequired]
        public int Bits { get; set; }

        [JsonPropertyName("tier")]
        [JsonRequired]
        public int Tier { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventMessageFragmentEmote {
        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("emote_set_id")]
        [JsonRequired]
        public string EmoteSetId { get; set; }

        [JsonPropertyName("owner_id")]
        [JsonRequired]
        public string OwnerId { get; set; }

        [JsonPropertyName("format")]
        [JsonRequired]
        public string[] Format { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventMessageFragmentMention {
        [JsonPropertyName("user_id")]
        [JsonRequired]
        public string UserId { get; set; }

        [JsonPropertyName("user_name")]
        [JsonRequired]
        public string UserName { get; set; }

        [JsonPropertyName("user_login")]
        [JsonRequired]
        public string UserLogin { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventBadge {
        [JsonPropertyName("set_id")]
        [JsonRequired]
        public string SetId { get; set; }

        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("info")]
        [JsonRequired]
        public string Info { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventCheer {
        [JsonPropertyName("bits")]
        [JsonRequired]
        public int Bits { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventReply {
        [JsonPropertyName("parent_message_id")]
        [JsonRequired]
        public string ParentMessageId { get; set; }

        [JsonPropertyName("parent_message_body")]
        [JsonRequired]
        public string ParentMessageBody { get; set; }

        [JsonPropertyName("parent_user_id")]
        [JsonRequired]
        public string ParentUserId { get; set; }

        [JsonPropertyName("parent_user_name")]
        [JsonRequired]
        public string ParentUserName { get; set; }

        [JsonPropertyName("parent_user_login")]
        [JsonRequired]
        public string ParentUserLogin { get; set; }

        [JsonPropertyName("thread_message_id")]
        [JsonRequired]
        public string ThreadMessageId { get; set; }

        [JsonPropertyName("thread_user_id")]
        [JsonRequired]
        public string ThreadUserId { get; set; }

        [JsonPropertyName("thread_user_name")]
        [JsonRequired]
        public string ThreadUserName { get; set; }

        [JsonPropertyName("thread_user_login")]
        [JsonRequired]
        public string ThreadUserLogin { get; set; }
    }

    public struct EventSubNotificationMessagePayloadEventSourceBadges {
        [JsonPropertyName("set_id")]
        [JsonRequired]
        public string SetId { get; set; }

        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("info")]
        [JsonRequired]
        public string Info { get; set; }
    }
}
