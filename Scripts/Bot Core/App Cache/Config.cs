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
        public readonly int SocketKeepaliveBuffer;
        public readonly int SocketKeepaliveTimeout;
        public readonly int TokenExpirationBuffer;

        public string MaskedSerialized => JsonSerializer.Serialize(new {
            AuthorizationPort = Scripts.Util.GetMasked(AuthorizationPort.ToString()),
            ChatterClientId,
            ChatterClientSecret = Scripts.Util.GetMasked(ChatterClientSecret),
            ChatterScope,
            CollectorClientId,
            CollectorClientSecret = Scripts.Util.GetMasked(CollectorClientSecret),
            CollectorScope,
            SocketKeepaliveBuffer,
            SocketKeepaliveTimeout,
            TokenExpirationBuffer,
        });

        public static async Task<Config?> Create() {
            var logPrefix = $"{nameof(Config)} | {nameof(Create)}";
            Logger.Info(logPrefix);

            string configText;
            try {
                configText = await File.ReadAllTextAsync(Constants.ConfigFilePath);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(File.ReadAllTextAsync)} threw: {e}.\n{nameof(Constants.ConfigFilePath)}: {Scripts.Util.GetMaskedPath(Constants.ConfigFilePath)}");
                return null;
            }

            ConfigData data;
            try {
                data = JsonSerializer.Deserialize<ConfigData>(configText);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(JsonSerializer.Deserialize)} threw: {e}.");
                return null;
            }

            return new(data);
        }

        private Config(ConfigData data) {
            Logger.Info($"{nameof(Config)} | Constructor\n{nameof(data)}: {data.MaskedSerialized}");

            AuthorizationPort = data.AuthorizationPort;
            ChatterClientId = data.ChatterClientId;
            ChatterClientSecret = data.ChatterClientSecret;
            ChatterScope = data.ChatterScope;
            CollectorClientId = data.CollectorClientId;
            CollectorClientSecret = data.CollectorClientSecret;
            CollectorScope = data.CollectorScope;
            SocketKeepaliveBuffer = data.SocketKeepaliveBuffer;
            SocketKeepaliveTimeout = data.SocketKeepaliveTimeout;
            TokenExpirationBuffer = data.TokenExpirationBuffer;
        }
    }
}
