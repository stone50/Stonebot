namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct ConfigData {
        [JsonPropertyName("authorizationPort")]
        public int AuthorizationPort { get; set; }
        [JsonPropertyName("botClientId")]
        public string BotClientId { get; set; }
        [JsonPropertyName("botClientSecret")]
        public string BotClientSecret { get; set; }
        [JsonPropertyName("broadcasterLogin")]
        public string BroadcasterLogin { get; set; }
        [JsonPropertyName("scope")]
        public string[] Scope { get; set; }
        [JsonPropertyName("socketKeepaliveBuffer")]
        public int SocketKeepaliveBuffer { get; set; }
        [JsonPropertyName("socketKeepaliveTimeout")]
        public int SocketKeepaliveTimeout { get; set; }
        [JsonPropertyName("tokenExpirationBuffer")]
        public int TokenExpirationBuffer { get; set; }
    }
}
