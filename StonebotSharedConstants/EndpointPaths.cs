namespace StonebotSharedConstants {
    public static class EndpointPaths {
        public const string GetStatus = "/status";
        public const string PostStop = "/stop";

        public const string GetTwitchStatus = "/twitch/status";
        public const string PostTwitchConnect = "/twitch/connect";
        public const string PostTwitchDisconnect = "/twitch/disconnect";

        public const string GetTwitchAuthStatus = "/twitch/auth/status";
        public const string PostTwitchAuthRefresh = "/twitch/auth/refresh";
        public const string GetTwitchAuthUrl = "/twitch/auth/url";
    }
}
