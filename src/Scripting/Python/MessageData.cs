namespace Stonebot.Scripting.Python {
    using Models.EventSubMessages;
    using System.Linq;

    public class MessageData(EventSubNotificationMessagePayloadEvent eventSubNotificationMessagePayloadEvent) {
        public readonly string broadcaster_id = eventSubNotificationMessagePayloadEvent.BroadcasterId;
        public readonly string broadcaster_user_name = eventSubNotificationMessagePayloadEvent.BroadcasterUserName;
        public readonly string broadcaster_login = eventSubNotificationMessagePayloadEvent.BroadcasterLogin;
        public readonly string chatter_id = eventSubNotificationMessagePayloadEvent.ChatterId;
        public readonly string chatter_user_name = eventSubNotificationMessagePayloadEvent.ChatterUserName;
        public readonly string chatter_login = eventSubNotificationMessagePayloadEvent.ChatterLogin;
        public readonly string message_id = eventSubNotificationMessagePayloadEvent.MessageId;
        public readonly EventMessage message = new(eventSubNotificationMessagePayloadEvent.Message);
        public readonly string message_type = eventSubNotificationMessagePayloadEvent.MessageType;
        public readonly EventBadge[] badges = [.. eventSubNotificationMessagePayloadEvent.Badges.Select(badge => new EventBadge(badge))];
        public readonly EventCheer? cheer = eventSubNotificationMessagePayloadEvent.Cheer is null ? null : new((EventSubNotificationMessagePayloadEventCheer)eventSubNotificationMessagePayloadEvent.Cheer);
        public readonly string color = eventSubNotificationMessagePayloadEvent.Color;
        public readonly EventReply? reply = eventSubNotificationMessagePayloadEvent.Reply is null ? null : new((EventSubNotificationMessagePayloadEventReply)eventSubNotificationMessagePayloadEvent.Reply);
        public readonly string? channel_points_custom_reward_id = eventSubNotificationMessagePayloadEvent.ChannelPointsCustomRewardId;
        public readonly string? source_broadcaster_id = eventSubNotificationMessagePayloadEvent.SourceBroadcasterId;
        public readonly string? source_broadcaster_user_name = eventSubNotificationMessagePayloadEvent.SourceBroadcasterUserName;
        public readonly string? source_broadcaster_login = eventSubNotificationMessagePayloadEvent.SourceBroadcasterLogin;
        public readonly string? source_message_id = eventSubNotificationMessagePayloadEvent.SourceMessageId;
        public readonly EventSourceBadges? source_badges = eventSubNotificationMessagePayloadEvent.SourceBadges is null ? null : new((EventSubNotificationMessagePayloadEventSourceBadges)eventSubNotificationMessagePayloadEvent.SourceBadges);
        public readonly bool? is_source_only = eventSubNotificationMessagePayloadEvent.IsSourceOnly;

        public class EventMessage(EventSubNotificationMessagePayloadEventMessage eventSubNotificationMessagePayloadEventMessage) {
            public readonly string text = eventSubNotificationMessagePayloadEventMessage.Text;
            public readonly EventMessageFragment[] fragments = [.. eventSubNotificationMessagePayloadEventMessage.Fragments.Select(fragment => new EventMessageFragment(fragment))];
        }

        public class EventMessageFragment(EventSubNotificationMessagePayloadEventMessageFragment eventSubNotificationMessagePayloadEventMessageFragment) {
            public readonly string type = eventSubNotificationMessagePayloadEventMessageFragment.Type;
            public readonly string text = eventSubNotificationMessagePayloadEventMessageFragment.Text;
            public readonly EventMessageFragmentCheermote? cheermote = eventSubNotificationMessagePayloadEventMessageFragment.Cheermote is null ? null : new((EventSubNotificationMessagePayloadEventMessageFragmentCheermote)eventSubNotificationMessagePayloadEventMessageFragment.Cheermote);
            public readonly EventMessageFragmentEmote? emote = eventSubNotificationMessagePayloadEventMessageFragment.Emote is null ? null : new((EventSubNotificationMessagePayloadEventMessageFragmentEmote)eventSubNotificationMessagePayloadEventMessageFragment.Emote);
            public readonly EventMessageFragmentMention? mention = eventSubNotificationMessagePayloadEventMessageFragment.Mention is null ? null : new((EventSubNotificationMessagePayloadEventMessageFragmentMention)eventSubNotificationMessagePayloadEventMessageFragment.Mention);
        }

        public class EventMessageFragmentCheermote(EventSubNotificationMessagePayloadEventMessageFragmentCheermote eventSubNotificationMessagePayloadEventMessageFragmentCheermote) {
            public readonly string prefix = eventSubNotificationMessagePayloadEventMessageFragmentCheermote.Prefix;
            public readonly int bits = eventSubNotificationMessagePayloadEventMessageFragmentCheermote.Bits;
            public readonly int tier = eventSubNotificationMessagePayloadEventMessageFragmentCheermote.Tier;
        }

        public class EventMessageFragmentEmote(EventSubNotificationMessagePayloadEventMessageFragmentEmote eventSubNotificationMessagePayloadEventMessageFragmentEmote) {
            public readonly string id = eventSubNotificationMessagePayloadEventMessageFragmentEmote.Id;
            public readonly string emote_set_id = eventSubNotificationMessagePayloadEventMessageFragmentEmote.EmoteSetId;
            public readonly string owner_id = eventSubNotificationMessagePayloadEventMessageFragmentEmote.OwnerId;
            public readonly string[] format = eventSubNotificationMessagePayloadEventMessageFragmentEmote.Format;
        }

        public class EventMessageFragmentMention(EventSubNotificationMessagePayloadEventMessageFragmentMention eventSubNotificationMessagePayloadEventMessageFragmentMention) {
            public readonly string user_id = eventSubNotificationMessagePayloadEventMessageFragmentMention.UserId;
            public readonly string user_name = eventSubNotificationMessagePayloadEventMessageFragmentMention.UserName;
            public readonly string user_login = eventSubNotificationMessagePayloadEventMessageFragmentMention.UserLogin;
        }

        public class EventBadge(EventSubNotificationMessagePayloadEventBadge eventSubNotificationMessagePayloadEventBadge) {
            public readonly string set_id = eventSubNotificationMessagePayloadEventBadge.SetId;
            public readonly string badge_id = eventSubNotificationMessagePayloadEventBadge.Id;
            public readonly string info = eventSubNotificationMessagePayloadEventBadge.Info;
        }

        public class EventCheer(EventSubNotificationMessagePayloadEventCheer eventSubNotificationMessagePayloadEventCheer) {
            public readonly int bits = eventSubNotificationMessagePayloadEventCheer.Bits;
        }

        public class EventReply(EventSubNotificationMessagePayloadEventReply eventSubNotificationMessagePayloadEventReply) {
            public readonly string parent_message_id = eventSubNotificationMessagePayloadEventReply.ParentMessageId;
            public readonly string parent_message_body = eventSubNotificationMessagePayloadEventReply.ParentMessageBody;
            public readonly string parent_user_id = eventSubNotificationMessagePayloadEventReply.ParentUserId;
            public readonly string parent_user_name = eventSubNotificationMessagePayloadEventReply.ParentUserName;
            public readonly string parent_user_login = eventSubNotificationMessagePayloadEventReply.ParentUserLogin;
            public readonly string thread_message_id = eventSubNotificationMessagePayloadEventReply.ThreadMessageId;
            public readonly string thread_user_id = eventSubNotificationMessagePayloadEventReply.ThreadUserId;
            public readonly string thread_user_name = eventSubNotificationMessagePayloadEventReply.ThreadUserName;
            public readonly string thread_user_login = eventSubNotificationMessagePayloadEventReply.ThreadUserLogin;
        }

        public class EventSourceBadges(EventSubNotificationMessagePayloadEventSourceBadges eventSubNotificationMessagePayloadEventSourceBadges) {
            public readonly string set_id = eventSubNotificationMessagePayloadEventSourceBadges.SetId;
            public readonly string badge_id = eventSubNotificationMessagePayloadEventSourceBadges.Id;
            public readonly string info = eventSubNotificationMessagePayloadEventSourceBadges.Info;
        }
    }
}
