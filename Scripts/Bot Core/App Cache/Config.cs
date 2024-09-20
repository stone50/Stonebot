namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class Config {
        public readonly int AuthorizationPort;
        public readonly string BroadcasterLogin;
        public readonly string ChatterClientId;
        public readonly string ChatterClientSecret;
        public readonly string[] ChatterScope;
        public readonly string ListenerClientId;
        public readonly string ListenerClientSecret;
        public readonly string[] ListenerScope;
        public readonly int SocketKeepaliveBuffer;
        public readonly int SocketKeepaliveTimeout;
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
            BroadcasterLogin = data.BroadcasterLogin;
            ChatterClientId = data.ChatterClientId;
            ChatterClientSecret = data.ChatterClientSecret;
            ChatterScope = data.ChatterScope;
            ListenerClientId = data.ListenerClientId;
            ListenerClientSecret = data.ListenerClientSecret;
            ListenerScope = data.ListenerScope;
            SocketKeepaliveBuffer = data.SocketKeepaliveBuffer;
            SocketKeepaliveTimeout = data.SocketKeepaliveTimeout;
            TokenExpirationBuffer = data.TokenExpirationBuffer;
        }
    }
}
