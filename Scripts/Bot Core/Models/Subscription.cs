namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct SimpleSubscriptionData {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonPropertyName("broadcaster_login")]
        public string BroadcasterLogin { get; set; }
        [JsonPropertyName("is_gift")]
        public bool IsGift { get; set; }
        [JsonPropertyName("tier")]
        public string Tier { get; set; }
    }

    internal struct SimpleSubscriptionsData {
        [JsonPropertyName("data")]
        public SimpleSubscriptionData[] Data { get; set; }
    }
}
