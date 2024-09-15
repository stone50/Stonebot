namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    public struct ConfigValues {
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
