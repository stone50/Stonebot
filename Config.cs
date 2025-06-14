namespace Stonebot {
    using Models.Data;
    using System.Text.Json;

    internal static class Config {
        public static string BroadcasterClientId = "";
        public static string BroadcasterClientSecret = "";
        public static string ChatterClientId = "";
        public static string ChatterClientSecret = "";
        public static int AuthorizationPort = Constants.AuthorizationPortDefault;
        public static int NumMaxLogFiles = Constants.NumMaxLogFilesDefault;
        public static int WebSocketConnectTimeoutSeconds = Constants.WebSocketConnectTimeoutSecondsDefault;
        public static int WebSocketKeepaliveTimeoutSeconds = Constants.WebSocketKeepaliveTimeoutSecondsDefault;
        public static int WebSocketKeepaliveTimeoutMarginSeconds = Constants.WebSocketKeepaliveTimeoutMarginSecondsDefault;

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
            WebSocketConnectTimeoutSeconds = configData.WebSocketConnectTimeoutSeconds;
            WebSocketKeepaliveTimeoutSeconds = configData.WebSocketKeepaliveTimeoutSeconds;
            WebSocketKeepaliveTimeoutMarginSeconds = configData.WebSocketKeepaliveTimeoutMarginSeconds;
        }

        public static void Save() {
            var contents = JsonSerializer.Serialize(new ConfigData() {
                BroadcasterClientId = BroadcasterClientId,
                BroadcasterClientSecret = BroadcasterClientSecret,
                ChatterClientId = ChatterClientId,
                ChatterClientSecret = ChatterClientSecret,
                AuthorizationPort = AuthorizationPort,
                NumMaxLogFiles = NumMaxLogFiles,
                WebSocketConnectTimeoutSeconds = WebSocketConnectTimeoutSeconds,
                WebSocketKeepaliveTimeoutSeconds = WebSocketKeepaliveTimeoutSeconds,
                WebSocketKeepaliveTimeoutMarginSeconds = WebSocketKeepaliveTimeoutMarginSeconds,
            }, JsonContext.Default.ConfigData);
            File.WriteAllText(Constants.ConfigFilePath, contents);
        }
    }
}
