namespace Stonebot {
    using Models.Data;
    using System.Text.Json;

    internal static class Config {
        public static string BroadcasterClientId = "";
        public static string BroadcasterClientSecret = "";
        public static string ChatterClientId = "";
        public static string ChatterClientSecret = "";
        public static int AuthorizationPort = 50505;
        public static int NumMaxLogFiles = 5;
        public static int WebSocketKeepaliveTimeoutSeconds = 10;
        public static int WebSocketConnectTimeoutSeconds = 3;

        public static void Init() {
            if (!File.Exists(Constants.ConfigFilePath)) {
                return;
            }

            var configFileContents = File.ReadAllText(Constants.ConfigFilePath);
            var configData = JsonSerializer.Deserialize(configFileContents, JsonContext.Default.ConfigData);
            BroadcasterClientId = configData.BroadcasterClientId;
            BroadcasterClientSecret = configData.BroadcasterClientSecret;
            ChatterClientId = configData.ChatterClientId;
            ChatterClientSecret = configData.ChatterClientSecret;
            AuthorizationPort = configData.AuthorizationPort;
            NumMaxLogFiles = configData.NumMaxLogFiles;
            WebSocketKeepaliveTimeoutSeconds = configData.WebSocketKeepaliveTimeoutSeconds;
            WebSocketConnectTimeoutSeconds = configData.WebSocketConnectTimeoutSeconds;
        }

        public static void Save() {
            var contents = JsonSerializer.Serialize(new ConfigData() {
                BroadcasterClientId = BroadcasterClientId,
                BroadcasterClientSecret = BroadcasterClientSecret,
                ChatterClientId = ChatterClientId,
                ChatterClientSecret = ChatterClientSecret,
                AuthorizationPort = AuthorizationPort,
                NumMaxLogFiles = NumMaxLogFiles,
                WebSocketKeepaliveTimeoutSeconds = WebSocketKeepaliveTimeoutSeconds,
                WebSocketConnectTimeoutSeconds = WebSocketConnectTimeoutSeconds,
            }, JsonContext.Default.ConfigData);
            File.WriteAllText(Constants.ConfigFilePath, contents);
        }
    }
}
