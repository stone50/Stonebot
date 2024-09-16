namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Text.Json.Serialization;

    internal class ConfigValues {
        // TODO
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
        [JsonPropertyName("tokenExpirationBuffer")]
        public int TokenExpirationBuffer { get; set; }

        public static async Task<ConfigValues?> GetConfigValues() {
            string jsonText;
            try {
                jsonText = await File.ReadAllTextAsync("config.json");
            } catch (Exception e) {
                GD.PushWarning($"Could not read text from config file: {e}.");
                return null;
            }

            ConfigValues values;
            try {
                values = JsonSerializer.Deserialize<ConfigValues>(jsonText);
            } catch (Exception e) {
                GD.PushWarning($"Could not deserialize config values: {e}.");
                return null;
            }

            return values;
        }
    }
}
