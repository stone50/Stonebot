namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal struct PostDeviceCodeResponse {
        [JsonPropertyName("device_code")]
        [JsonRequired]
        public string DeviceCode { get; set; }

        [JsonPropertyName("user_code")]
        [JsonRequired]
        public string UserCode { get; set; }

        [JsonPropertyName("verification_uri")]
        [JsonRequired]
        public string VerificationUri { get; set; }

        [JsonPropertyName("expires_in")]
        [JsonRequired]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("interval")]
        [JsonRequired]
        public int Interval { get; set; }
    }
}
