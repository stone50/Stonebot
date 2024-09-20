namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct ConfigData {
        [JsonPropertyName("authorizationPort")]
        public int AuthorizationPort { get; set; }
        [JsonPropertyName("chatterClientId")]
        public string ChatterClientId { get; set; }
        [JsonPropertyName("chatterClientSecret")]
        public string ChatterClientSecret { get; set; }
        [JsonPropertyName("chatterScope")]
        public string[] ChatterScope { get; set; }
        [JsonPropertyName("collectorClientId")]
        public string CollectorClientId { get; set; }
        [JsonPropertyName("collectorClientSecret")]
        public string CollectorClientSecret { get; set; }
        [JsonPropertyName("collectorScope")]
        public string[] CollectorScope { get; set; }
        [JsonPropertyName("socketKeepaliveBuffer")]
        public int SocketKeepaliveBuffer { get; set; }
        [JsonPropertyName("socketKeepaliveTimeout")]
        public int SocketKeepaliveTimeout { get; set; }
        [JsonPropertyName("tokenExpirationBuffer")]
        public int TokenExpirationBuffer { get; set; }
    }
}
