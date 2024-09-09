namespace StoneBot.Scripts.Middleware {
    using Godot;
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static class Broadcaster {
        public struct UserData {
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

        public struct UsersData {
            [JsonPropertyName("data")]
            public UserData[] Data { get; set; }
        }

        public static async Task<UserData?> GetBroadcasterData(HttpClient client) {
            if (Configuration.TwitchBroadcasterLogin is null) {
                GD.PushWarning("Cannot get broadcaster data because TwitchBroadcasterLogin is null.");
                return null;
            }

            var response = await TwitchAPI.GetUsers(client, null, new[] { Configuration.TwitchBroadcasterLogin });
            if (response is null) {
                GD.PushWarning("Cannot get broadcaster data because GetUsers failed.");
                return null;
            }

            string responseString;
            try {
                responseString = await response.Content.ReadAsStringAsync();
            } catch (Exception e) {
                GD.PushWarning($"Could not read response string: {e}.");
                return null;
            }

            if (!response.IsSuccessStatusCode) {
                GD.PushWarning($"Cannot get broadcaster data because GetUsers failed: {responseString}.");
                return null;
            }

            UsersData usersData;
            try {
                usersData = JsonSerializer.Deserialize<UsersData>(responseString);
            } catch (Exception e) {
                GD.PushWarning($"Could not parse response json: {e}.");
                return null;
            }

            if (usersData.Data.Length == 0) {
                GD.PushWarning($"Cannot get broadcaster data because GetUsers found no users.");
                return null;
            }

            return usersData.Data[0];
        }
    }
}
