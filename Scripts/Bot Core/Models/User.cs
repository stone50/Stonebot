namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct UserData {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("login")]
        public string Login { get; set; }
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("broadcaster_type")]
        public string BroadcasterType { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("profile_image_url")]
        public string ProfileImageURL { get; set; }
        [JsonPropertyName("offline_image_url")]
        public string OfflineImageURL { get; set; }
        [JsonPropertyName("view_count")]
        public int ViewCount { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }

    internal struct UsersData {
        [JsonPropertyName("data")]
        public UserData[] Data { get; set; }
    }

    internal struct SimpleUserData {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }
    }
}
