namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class Config {
        public readonly int AuthorizationPort;
        public readonly string BotClientId;
        public readonly string BotClientSecret;
        public readonly string BroadcasterLogin;
        public readonly string[] Scope;
        public readonly int TokenExpirationBuffer;

        public static async Task<Config?> Create() {
            string configText;
            try {
                configText = await File.ReadAllTextAsync("config.json");
            } catch (Exception e) {
                GD.PushWarning($"Cannot create config because File.ReadAllTextAsync failed: {e}.");
                return null;
            }

            ConfigData data;
            try {
                data = JsonSerializer.Deserialize<ConfigData>(configText);
            } catch (Exception e) {
                GD.PushWarning($"Could not deserialize config values: {e}.");
                return null;
            }

            return new(data);
        }

        private Config(ConfigData data) {
            AuthorizationPort = data.AuthorizationPort;
            BotClientId = data.BotClientId;
            BotClientSecret = data.BotClientSecret;
            BroadcasterLogin = data.BroadcasterLogin;
            Scope = data.Scope;
            TokenExpirationBuffer = data.TokenExpirationBuffer;
        }
    }
}
