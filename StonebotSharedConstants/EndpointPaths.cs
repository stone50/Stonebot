namespace StonebotSharedConstants {
    public static class EndpointPaths {
        public const string GetHealth = "/health";
        public const string PostStop = "/stop";
        public const string PostSubscriber = "/subscriber";
        public const string GetSubscriber = "/subscriber/{subscriberId}";
        public const string DeleteSubscriber = "/subscriber/{subscriberId}";
        public const string PostAuthTwitchStart = "/auth/twitch/start";
        public const string GetAuthTwitch = "/auth/twitch";
        public const string PostAuthTwitchRefresh = "/auth/twitch/refresh";
        public const string PostConfigLoad = "/config/load";
        public const string PatchConfigSet = "/config/set";
    }
}
