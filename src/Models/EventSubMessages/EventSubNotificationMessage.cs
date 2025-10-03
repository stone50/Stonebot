namespace Stonebot.Models.EventSubMessages {
    using System.Text.Json.Serialization;

    internal class EventSubNotificationMessage {
        [JsonPropertyName("metadata")]
        public required EventSubNotificationMessageMetadata Metadata { get; init; }

        [JsonPropertyName("payload")]
        public required EventSubNotificationMessagePayload Payload { get; init; }
    }

    internal class EventSubNotificationMessageMetadata {
        [JsonPropertyName("message_id")]
        public required string MessageId { get; init; }

        [JsonPropertyName("message_type")]
        public required string MessageType { get; init; }

        [JsonPropertyName("message_timestamp")]
        public required string MessageTimestamp { get; init; }

        [JsonPropertyName("subscription_type")]
        public required string SubscriptionType { get; init; }

        [JsonPropertyName("subscription_version")]
        public required string SubscriptionVersion { get; init; }
    }

    internal class EventSubNotificationMessagePayload {
        [JsonPropertyName("subscription")]
        public required EventSubNotificationMessagePayloadSubscription Subscription { get; init; }

        [JsonPropertyName("event")]
        public required EventSubNotificationMessagePayloadEvent Event { get; init; }
    }

    internal class EventSubNotificationMessagePayloadSubscription {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("status")]
        public required string Status { get; init; }

        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("version")]
        public required string Version { get; init; }

        [JsonPropertyName("cost")]
        public required int Cost { get; init; }

        [JsonPropertyName("condition")]
        public required EventSubNotificationMessagePayloadSubscriptionCondition Condition { get; init; }

        [JsonPropertyName("transport")]
        public required EventSubNotificationMessagePayloadSubscriptionTransport Transport { get; init; }

        [JsonPropertyName("created_at")]
        public required string CreatedAt { get; init; }
    }

    internal class EventSubNotificationMessagePayloadSubscriptionCondition {
        [JsonPropertyName("broadcaster_user_id")]
        public required string BroadcasterId { get; init; }

        [JsonPropertyName("user_id")]
        public required string UserId { get; init; }
    }

    internal class EventSubNotificationMessagePayloadSubscriptionTransport {
        [JsonPropertyName("method")]
        public required string Method { get; init; }

        [JsonPropertyName("session_id")]
        public required string SessionId { get; init; }
    }

    public class EventSubNotificationMessagePayloadEvent {
        [JsonPropertyName("broadcaster_user_id")]
        public required string BroadcasterId { get; init; }

        [JsonPropertyName("broadcaster_user_name")]
        public required string BroadcasterUserName { get; init; }

        [JsonPropertyName("broadcaster_user_login")]
        public required string BroadcasterLogin { get; init; }

        [JsonPropertyName("chatter_user_id")]
        public required string ChatterId { get; init; }

        [JsonPropertyName("chatter_user_name")]
        public required string ChatterUserName { get; init; }

        [JsonPropertyName("chatter_user_login")]
        public required string ChatterLogin { get; init; }

        [JsonPropertyName("message_id")]
        public required string MessageId { get; init; }

        [JsonPropertyName("message")]
        public required EventSubNotificationMessagePayloadEventMessage Message { get; init; }

        [JsonPropertyName("message_type")]
        public required string MessageType { get; init; }

        [JsonPropertyName("badges")]
        public required EventSubNotificationMessagePayloadEventBadge[] Badges { get; init; }

        [JsonPropertyName("cheer")]
        public EventSubNotificationMessagePayloadEventCheer? Cheer { get; init; }

        [JsonPropertyName("color")]
        public required string Color { get; init; }

        [JsonPropertyName("reply")]
        public EventSubNotificationMessagePayloadEventReply? Reply { get; init; }

        [JsonPropertyName("channel_points_custom_reward_id")]
        public string? ChannelPointsCustomRewardId { get; init; }

        [JsonPropertyName("source_broadcaster_user_id")]
        public string? SourceBroadcasterId { get; init; }

        [JsonPropertyName("source_broadcaster_user_name")]
        public string? SourceBroadcasterUserName { get; init; }

        [JsonPropertyName("source_broadcaster_user_login")]
        public string? SourceBroadcasterLogin { get; init; }

        [JsonPropertyName("source_message_id")]
        public string? SourceMessageId { get; init; }

        [JsonPropertyName("source_badges")]
        public EventSubNotificationMessagePayloadEventSourceBadges? SourceBadges { get; init; }

        [JsonPropertyName("is_source_only")]
        public bool? IsSourceOnly { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventMessage {
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        [JsonPropertyName("fragments")]
        public required EventSubNotificationMessagePayloadEventMessageFragment[] Fragments { get; set; }
    }

    public class EventSubNotificationMessagePayloadEventMessageFragment {
        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("text")]
        public required string Text { get; set; }

        [JsonPropertyName("cheermote")]
        public EventSubNotificationMessagePayloadEventMessageFragmentCheermote? Cheermote { get; init; }

        [JsonPropertyName("emote")]
        public EventSubNotificationMessagePayloadEventMessageFragmentEmote? Emote { get; init; }

        [JsonPropertyName("mention")]
        public EventSubNotificationMessagePayloadEventMessageFragmentMention? Mention { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventMessageFragmentCheermote {
        [JsonPropertyName("prefix")]
        public required string Prefix { get; init; }

        [JsonPropertyName("bits")]
        public required int Bits { get; init; }

        [JsonPropertyName("tier")]
        public required int Tier { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventMessageFragmentEmote {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("emote_set_id")]
        public required string EmoteSetId { get; init; }

        [JsonPropertyName("owner_id")]
        public required string OwnerId { get; init; }

        [JsonPropertyName("format")]
        public required string[] Format { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventMessageFragmentMention {
        [JsonPropertyName("user_id")]
        public required string UserId { get; init; }

        [JsonPropertyName("user_name")]
        public required string UserName { get; init; }

        [JsonPropertyName("user_login")]
        public required string UserLogin { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventBadge {
        [JsonPropertyName("set_id")]
        public required string SetId { get; init; }

        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("info")]
        public required string Info { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventCheer {
        [JsonPropertyName("bits")]
        public required int Bits { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventReply {
        [JsonPropertyName("parent_message_id")]
        public required string ParentMessageId { get; init; }

        [JsonPropertyName("parent_message_body")]
        public required string ParentMessageBody { get; init; }

        [JsonPropertyName("parent_user_id")]
        public required string ParentUserId { get; init; }

        [JsonPropertyName("parent_user_name")]
        public required string ParentUserName { get; init; }

        [JsonPropertyName("parent_user_login")]
        public required string ParentUserLogin { get; init; }

        [JsonPropertyName("thread_message_id")]
        public required string ThreadMessageId { get; init; }

        [JsonPropertyName("thread_user_id")]
        public required string ThreadUserId { get; init; }

        [JsonPropertyName("thread_user_name")]
        public required string ThreadUserName { get; init; }

        [JsonPropertyName("thread_user_login")]
        public required string ThreadUserLogin { get; init; }
    }

    public class EventSubNotificationMessagePayloadEventSourceBadges {
        [JsonPropertyName("set_id")]
        public required string SetId { get; init; }

        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("info")]
        public required string Info { get; init; }
    }
}
