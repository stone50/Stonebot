namespace Stonebot {
    using Models.Data;
    using System.Text.Json;

    internal static class Config {
        public static string ChatterClientId = "";
        public static string ChatterClientSecret = "";
        public static string[] ChatterScopes = ["user:write:chat", "user:read:chat"];
        public static string BroadcasterClientId = "";
        public static string BroadcasterClientSecret = "";
        public static string[] BroadcasterScopes = ["channel:read:vips", "channel:read:subscriptions", "moderation:read"];
        public static int AuthorizationPort = 50505;
        public static int NumMaxLogFiles = 5;
        public static int AccessTokenExpirationMarginMillis = 300;
        public static int SocketKeepaliveTimeoutSeconds = 10;

        public static void Init() {
            if (!File.Exists(Constants.ConfigFilePath)) {
                return;
            }

            var configFileContents = File.ReadAllText(Constants.ConfigFilePath);
            var configData = JsonSerializer.Deserialize(configFileContents, JsonContext.Default.ConfigData);
            ChatterClientId = configData.ChatterClientId;
            ChatterClientSecret = configData.ChatterClientSecret;
            ChatterScopes = configData.ChatterScopes;
            BroadcasterClientId = configData.BroadcasterClientId;
            BroadcasterClientSecret = configData.BroadcasterClientSecret;
            BroadcasterScopes = configData.BroadcasterScopes;
            AuthorizationPort = configData.AuthorizationPort;
            NumMaxLogFiles = configData.NumMaxLogFiles;
            AccessTokenExpirationMarginMillis = configData.AccessTokenExpirationMarginMillis;
            SocketKeepaliveTimeoutSeconds = configData.SocketKeepaliveTimeoutSeconds;
        }

        public static void Save() {
            var contents = JsonSerializer.Serialize(new ConfigData() {
                ChatterClientId = ChatterClientId,
                ChatterClientSecret = ChatterClientSecret,
                ChatterScopes = ChatterScopes,
                BroadcasterClientId = BroadcasterClientId,
                BroadcasterClientSecret = BroadcasterClientSecret,
                BroadcasterScopes = BroadcasterScopes,
                AuthorizationPort = AuthorizationPort,
                NumMaxLogFiles = NumMaxLogFiles,
                AccessTokenExpirationMarginMillis = AccessTokenExpirationMarginMillis,
                SocketKeepaliveTimeoutSeconds = SocketKeepaliveTimeoutSeconds,
            }, JsonContext.Default.ConfigData);
            File.WriteAllText(Constants.ConfigFilePath, contents);
        }
    }
}
