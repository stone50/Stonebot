namespace Stonebot.Scripts.Bot_Core.Models.EventSub {
    using System.Text.Json.Serialization;

    internal struct ChannelChatMessageEventMessageFragmentCheermote {
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }
        [JsonPropertyName("bits")]
        public int Bits { get; set; }
        [JsonPropertyName("tier")]
        public int Tier { get; set; }
    }

    internal struct ChannelChatMessageEventMessageFragmentEmote {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("emote_set_id")]
        public string EmoteSetId { get; set; }
        [JsonPropertyName("owner_id")]
        public string OwnerId { get; set; }
        [JsonPropertyName("format")]
        public string[] Format { get; set; }
    }

    internal struct ChannelChatMessageEventMessageFragmentMention {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }
    }

    internal struct ChannelChatMessageEventMessageFragment {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("cheermote")]
        public ChannelChatMessageEventMessageFragmentCheermote? Cheermote { get; set; }

        [JsonPropertyName("emote")]
        public ChannelChatMessageEventMessageFragmentEmote? Emote { get; set; }

        [JsonPropertyName("mention")]
        public ChannelChatMessageEventMessageFragmentMention? Mention { get; set; }
    }

    internal struct ChannelChatMessageEventMessage {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("fragments")]
        public ChannelChatMessageEventMessageFragment[] Fragments { get; set; }
    }

    internal struct ChannelChatMessageEventBadge {
        [JsonPropertyName("set_id")]
        public string SetId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("info")]
        public string Info { get; set; }
    }

    internal struct ChannelChatMessageEventCheer {
        [JsonPropertyName("bits")]
        public int Bits { get; set; }
    }

    internal struct ChannelChatMessageEventReply {
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

    internal struct ChannelChatMessageEventSourceBadge {
        [JsonPropertyName("set_id")]
        public string SetId { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("info")]
        public string Info { get; set; }
    }

    internal struct ChannelChatMessageEvent {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterUserId { get; set; }

        [JsonPropertyName("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; set; }

        [JsonPropertyName("broadcaster_user_name")]
        public string BroadcasterUserName { get; set; }

        [JsonPropertyName("chatter_user_id")]
        public string ChatterUserId { get; set; }

        [JsonPropertyName("chatter_user_login")]
        public string ChatterUserLogin { get; set; }

        [JsonPropertyName("chatter_user_name")]
        public string ChatterUserName { get; set; }

        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }

        [JsonPropertyName("message")]
        public ChannelChatMessageEventMessage Message { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("badges")]
        public ChannelChatMessageEventBadge[] Badges { get; set; }

        [JsonPropertyName("message_type")]
        public string MessageType { get; set; }

        [JsonPropertyName("cheer")]
        public ChannelChatMessageEventCheer? Cheer { get; set; }

        [JsonPropertyName("reply")]
        public ChannelChatMessageEventReply? Reply { get; set; }

        [JsonPropertyName("channel_points_custom_reward_id")]
        public string? ChannelPointsCustomRewardId { get; set; }

        [JsonPropertyName("source_broadcaster_user_id")]
        public string? SourceBroadcasterUserId { get; set; }

        [JsonPropertyName("source_broadcaster_user_login")]
        public string? SourceBroadcasterUserLogin { get; set; }

        [JsonPropertyName("source_broadcaster_user_name")]
        public string? SourceBroadcasterUserName { get; set; }

        [JsonPropertyName("source_message_id")]
        public string? SourceMessageId { get; set; }

        [JsonPropertyName("source_badges")]
        public ChannelChatMessageEventSourceBadge[]? SourceBadges { get; set; }
    }
}
