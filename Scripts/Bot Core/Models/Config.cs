namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct ConfigData {
        [JsonPropertyName("authorizationPort")]
        public int AuthorizationPort { get; set; }
        [JsonPropertyName("broadcasterLogin")]
        public string BroadcasterLogin { get; set; }
        [JsonPropertyName("chatterClientId")]
        public string ChatterClientId { get; set; }
        [JsonPropertyName("chatterClientSecret")]
        public string ChatterClientSecret { get; set; }
        [JsonPropertyName("chatterScope")]
        public string[] ChatterScope { get; set; }
        [JsonPropertyName("listenerClientId")]
        public string ListenerClientId { get; set; }
        [JsonPropertyName("listenerClientSecret")]
        public string ListenerClientSecret { get; set; }
        [JsonPropertyName("listenerScope")]
        public string[] ListenerScope { get; set; }
        [JsonPropertyName("socketKeepaliveBuffer")]
        public int SocketKeepaliveBuffer { get; set; }
        [JsonPropertyName("socketKeepaliveTimeout")]
        public int SocketKeepaliveTimeout { get; set; }
        [JsonPropertyName("tokenExpirationBuffer")]
        public int TokenExpirationBuffer { get; set; }
    }
}
