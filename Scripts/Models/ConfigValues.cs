namespace StoneBot.Scripts.Models {
    using System.Text.Json.Serialization;

    public struct ConfigValues {
        [JsonPropertyName("serverMaxRetries")]
        public int ServerMaxRetries { get; set; }
        [JsonPropertyName("authorizationPort")]
        public int AuthorizationPort { get; set; }
        [JsonPropertyName("botClientId")]
        public string BotClientId { get; set; }
        [JsonPropertyName("botClientSecret")]
        public string BotClientSecret { get; set; }
        [JsonPropertyName("broadcasterLogin")]
        public string BroadcasterLogin { get; set; }
    }
}
