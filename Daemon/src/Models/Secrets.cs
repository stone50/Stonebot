namespace StonebotDaemon.Models {
    internal sealed class Secrets {
        public string TwitchClientSecret { get; set; } = string.Empty;
        public string TwitchAccessToken { get; set; } = string.Empty;
        public string TwitchRefreshToken { get; set; } = string.Empty;
        public string LocalApiKey { get; set; } = string.Empty;
    }
}
