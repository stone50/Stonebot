namespace StonebotDaemon.Models {
    internal sealed class Config {
        public int Port { get; set; } = StonebotSharedConstants.Port.Default;
        public string TwitchBotUsername { get; set; } = string.Empty;
        public string TwitchBroadcasterChannel { get; set; } = string.Empty;
        public string TwitchClientId { get; set; } = string.Empty;
    }
}