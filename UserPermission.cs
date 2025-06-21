namespace Stonebot {
    using Twitch;

    public static class UserPermission {
        public enum Level {
            Viewer,
            VIP,
            Tier1Sub,
            Tier2Sub,
            Tier3Sub,
            Moderator,
            Broadcaster
        }

        public static Level GetHighestLevel(string userId, CancellationToken cancellationToken) {
            if (userId == Cache.BroadcasterAuthorizationData!.UserId) {
                return Level.Broadcaster;
            }

            if (User.GetIsModerator(userId, cancellationToken)) {
                return Level.Moderator;
            }

            var subTier = User.GetSubscriptionTier(userId, cancellationToken);
            if (subTier != -1) {
                switch (subTier) {
                    case 1:
                        return Level.Tier1Sub;
                    case 2:
                        return Level.Tier2Sub;
                    case 3:
                        return Level.Tier3Sub;
                }
            }

            return User.GetIsVIP(userId, cancellationToken) ? Level.VIP : Level.Viewer;
        }
    }
}
