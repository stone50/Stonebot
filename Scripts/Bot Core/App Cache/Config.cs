namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class Config {
        public readonly int AuthorizationPort;
        public readonly string ChatterClientId;
        public readonly string ChatterClientSecret;
        public readonly string[] ChatterScope;
        public readonly string CollectorClientId;
        public readonly string CollectorClientSecret;
        public readonly string[] CollectorScope;
        public readonly int DisplayLogLimit;
        public readonly int SocketKeepaliveBuffer;
        public readonly int SocketKeepaliveTimeout;
        public readonly int TokenExpirationBuffer;

        public static async Task<Config?> Create() {
            Logger.Info("Creating config.");

            string configText;
            try {
                configText = await File.ReadAllTextAsync(Constants.ConfigFilePath);
            } catch (Exception e) {
                Logger.Warning($"Could not create config because file read all text attempt failed: {e}. Context value: {Constants.ConfigFilePath}.");
                return null;
            }

            ConfigData data;
            try {
                data = JsonSerializer.Deserialize<ConfigData>(configText);
            } catch (Exception e) {
                Logger.Warning($"Could not create config because json serializer deserialize attempt failed: {e}. Context value: {configText}.");
                return null;
            }

            return new(data);
        }

        private Config(ConfigData data) {
            AuthorizationPort = data.AuthorizationPort;
            ChatterClientId = data.ChatterClientId;
            ChatterClientSecret = data.ChatterClientSecret;
            ChatterScope = data.ChatterScope;
            CollectorClientId = data.CollectorClientId;
            CollectorClientSecret = data.CollectorClientSecret;
            CollectorScope = data.CollectorScope;
            DisplayLogLimit = data.DisplayLogLimit;
            SocketKeepaliveBuffer = data.SocketKeepaliveBuffer;
            SocketKeepaliveTimeout = data.SocketKeepaliveTimeout;
            TokenExpirationBuffer = data.TokenExpirationBuffer;
        }
    }
}
