namespace Stonebot.Scripts.Bot_Core.Models {
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Util = Scripts.Util;

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

        public readonly string MaskedSerialized => JsonSerializer.Serialize(new {
            AuthorizationPort = Util.GetMasked(AuthorizationPort.ToString()),
            ChatterClientId = Util.GetMasked(ChatterClientId),
            ChatterClientSecret = Util.GetMasked(ChatterClientSecret),
            ChatterScope,
            CollectorClientId = Util.GetMasked(CollectorClientId),
            CollectorClientSecret = Util.GetMasked(CollectorClientSecret),
            CollectorScope,
            SocketKeepaliveBuffer,
            SocketKeepaliveTimeout,
            TokenExpirationBuffer,
        });
    }
}
