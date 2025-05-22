namespace Stonebot.Models {
    using System.Text.Json.Serialization;

    internal struct EventSubNotificationMessage {
        [JsonPropertyName("metadata")]
        public EventSubNotificationMessageMetadata Metadata { get; set; }
        [JsonPropertyName("payload")]
        public EventSubNotificationMessagePayload Payload { get; set; }
    }

    internal struct EventSubNotificationMessageMetadata {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("message_timestamp")]
        public string MessageTimestamp { get; set; }
        [JsonPropertyName("subscription_type")]
        public string SubscriptionType { get; set; }
        [JsonPropertyName("subscription_version")]
        public string SubscriptionVersion { get; set; }
    }

    internal struct EventSubNotificationMessagePayload {
        [JsonPropertyName("subscription")]
        public EventSubNotificationMessagePayloadSubscription Subscription { get; set; }
        [JsonPropertyName("event")]
        public EventSubNotificationMessagePayloadEvent Event { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadSubscription {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("cost")]
        public int Cost { get; set; }
        [JsonPropertyName("condition")]
        public ChannelChatMessageEventSubCondition Condition { get; set; }
        [JsonPropertyName("transport")]
        public EventSubNotificationMessagePayloadSubscriptionTransport Transport { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadSubscriptionTransport {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEvent {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; set; }
        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterLogin { get; set; }
        [JsonPropertyName("chatter_user_id")]
        public string ChatterId { get; set; }
        [JsonPropertyName("chatter_user_name")]
        public string ChatterUserName { get; set; }
        [JsonPropertyName("chatter_user_login")]
        public string ChatterLogin { get; set; }
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }
        [JsonPropertyName("message")]
        public EventSubNotificationMessagePayloadEventMessage Message { get; set; }
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }
        [JsonPropertyName("badges")]
        public EventSubNotificationMessagePayloadEventBadge[] Badges { get; set; }
        [JsonPropertyName("cheer")]
        public EventSubNotificationMessagePayloadEventCheer? Cheer { get; set; }
        [JsonPropertyName("color")]
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

    internal struct EventSubNotificationMessagePayloadEventMessage {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("fragments")]
        public EventSubNotificationMessagePayloadEventMessageFragment[] Fragments { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventMessageFragment {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("cheermote")]
        public EventSubNotificationMessagePayloadEventMessageFragmentCheermote? Cheermote { get; set; }
        [JsonPropertyName("emote")]
        public EventSubNotificationMessagePayloadEventMessageFragmentEmote? Emote { get; set; }
        [JsonPropertyName("mention")]
        public EventSubNotificationMessagePayloadEventMessageFragmentMention? Mention { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventMessageFragmentCheermote {
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }
        [JsonPropertyName("bits")]
        public int Bits { get; set; }
        [JsonPropertyName("tier")]
        public int Tier { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventMessageFragmentEmote {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("emote_set_id")]
        public string EmoteSetId { get; set; }
        [JsonPropertyName("owner_id")]
        public string OwnerId { get; set; }
        [JsonPropertyName("format")]
        public string[] Format { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventMessageFragmentMention {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventBadge {
        [JsonPropertyName("set_id")]
        public string SetId { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("info")]
        public string Info { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventCheer {
        [JsonPropertyName("bits")]
        public int Bits { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventReply {
        [JsonPropertyName("parent_message_id")]
        public string ParentMessageId { get; set; }
        [JsonPropertyName("parent_message_body")]
        public string ParentMessageBody { get; set; }
        [JsonPropertyName("parent_user_id")]
        public string ParentUserId { get; set; }
        [JsonPropertyName("parent_user_name")]
        public string ParentUserName { get; set; }
        [JsonPropertyName("parent_user_login")]
        public string ParentUserLogin { get; set; }
        [JsonPropertyName("thread_message_id")]
        public string ThreadMessageId { get; set; }
        [JsonPropertyName("thread_user_id")]
        public string ThreadUserId { get; set; }
        [JsonPropertyName("thread_user_name")]
        public string ThreadUserName { get; set; }
        [JsonPropertyName("thread_user_login")]
        public string ThreadUserLogin { get; set; }
    }

    internal struct EventSubNotificationMessagePayloadEventSourceBadges {
        [JsonPropertyName("set_id")]
        public string SetId { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("info")]
        public string Info { get; set; }
    }
}
