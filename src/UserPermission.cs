namespace Stonebot {
    using Models.EventSubMessages;

    public static class UserPermission {
        public enum Level {
            Viewer,
            Subscriber,
            VIP,
            Moderator,
            Broadcaster
        }

        public static Level GetHighestLevel(EventSubNotificationMessagePayloadEventBadge[] badges) {
            foreach (var badge in badges) {
                switch (badge.SetId) {
                    case "broadcaster":
                        return Level.Broadcaster;
                    case "moderator":
                        return Level.Moderator;
                    case "vip":
                        return Level.VIP;
                    case "subscriber":
                        return Level.Subscriber;
                    case "founder":
                        return Level.Subscriber;
                }
            }

            return Level.Viewer;
        }
    }
}
