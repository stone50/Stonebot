namespace StonebotSharedConstants {
    public static class EndpointPaths {
        public const string GetHealth = "/health";
        public const string PostStop = "/stop";
        public const string PostSubscriber = "/subscriber";
        public const string GetSubscriber = "/subscriber/{subscriberId}";
        public const string DeleteSubscriber = "/subscriber/{subscriberId}";
        public const string PostTwitchAuthStart = "/twitch/auth/start";
        public const string GetTwitchAuth = "/twitch/auth";
        public const string PostTwitchAuthRefresh = "/twitch/auth/refresh";
        public const string PostConfigLoad = "/config/load";
        public const string PatchConfigSet = "/config/set";
        public const string PostTwitchConfigureClient = "/twitch/configure";
        public const string PostTwitchConnect = "/twitch/connect";
        public const string PostTwitchDisconnect = "/twitch/disconnect";
    }
}
