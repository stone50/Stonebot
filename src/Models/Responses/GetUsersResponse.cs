namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal class GetUsersResponse {
        [JsonPropertyName("data")]
        public required GetUsersResponseDataPoint[] Data { get; init; }
    }

    internal class GetUsersResponseDataPoint {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("login")]
        public required string Login { get; init; }

        [JsonPropertyName("display_name")]
        public required string DisplayName { get; init; }

        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("broadcaster_type")]
        public required string BroadcasterType { get; init; }

        [JsonPropertyName("description")]
        public required string Description { get; init; }

        [JsonPropertyName("profile_image_url")]
        public required string ProfileImageUrl { get; init; }

        [JsonPropertyName("offline_image_url")]
        public required string OfflineImageUrl { get; init; }

        [JsonPropertyName("view_count")]
        public required int ViewCount { get; init; }

        [JsonPropertyName("created_at")]
        public required string CreatedAt { get; init; }
    }
}
