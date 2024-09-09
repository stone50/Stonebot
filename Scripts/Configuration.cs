namespace StoneBot.Scripts {
    using Godot;
    using System;
    using System.IO;
    using System.Text.Json;

    internal static class Configuration {
        private struct ConfigValues {
            public int? ServerMaxRetries { get; set; }
            public int? AuthorizationServerPort { get; set; }
            public string? TwitchStoneBotClientId { get; set; }
            public string? TwitchStoneBotClientSecret { get; set; }
            public string? TwitchBroadcasterLogin { get; set; }
        }

        private static ConfigValues Values;

        public static int? ServerMaxRetries => Values.ServerMaxRetries;
        public static int? AuthorizationServerPort => Values.AuthorizationServerPort;
        public static string? TwitchStoneBotClientId => Values.TwitchStoneBotClientId;
        public static string? TwitchStoneBotClientSecret => Values.TwitchStoneBotClientSecret;
        public static string? TwitchBroadcasterLogin => Values.TwitchBroadcasterLogin;

        public static bool Init() {
            string jsonText;
            try {
                jsonText = File.ReadAllText("config.json");
            } catch (Exception e) {
                GD.PushWarning($"Could not read text from config file: {e}.");
                return false;
            }

            try {
                Values = JsonSerializer.Deserialize<ConfigValues>(jsonText);
            } catch (Exception e) {
                GD.PushWarning($"Could not deserialize config values: {e}.");
                return false;
            }

            return true;
        }
    }
}
