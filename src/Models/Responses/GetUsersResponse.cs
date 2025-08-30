namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal struct GetUsersResponse {
        [JsonPropertyName("data")]
        [JsonRequired]
        public GetUsersResponseDataPoint[] Data { get; set; }
    }

    internal struct GetUsersResponseDataPoint {
        [JsonPropertyName("id")]
        [JsonRequired]
        public string Id { get; set; }

        [JsonPropertyName("login")]
        [JsonRequired]
        public string Login { get; set; }

        [JsonPropertyName("display_name")]
        [JsonRequired]
        public string DisplayName { get; set; }

        [JsonPropertyName("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonPropertyName("broadcaster_type")]
        [JsonRequired]
        public string BroadcasterType { get; set; }

        [JsonPropertyName("description")]
        [JsonRequired]
        public string Description { get; set; }

        [JsonPropertyName("profile_image_url")]
        [JsonRequired]
        public string ProfileImageUrl { get; set; }

        [JsonPropertyName("offline_image_url")]
        [JsonRequired]
        public string OfflineImageUrl { get; set; }

        [JsonPropertyName("view_count")]
        [JsonRequired]
        public int ViewCount { get; set; }

        [JsonPropertyName("created_at")]
        [JsonRequired]
        public string CreatedAt { get; set; }
    }
}