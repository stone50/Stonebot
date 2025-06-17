namespace Stonebot {
    using System.IO;

    internal static class Constants {
        public static readonly string AppDataPath = new(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stonebot"));
        public static readonly string LogsPath = new(Path.Join(AppDataPath, "logs"));
        public static readonly string ConfigFilePath = new(Path.Join(AppDataPath, "config.json"));
        public static readonly string CacheFilePath = new(Path.Join(AppDataPath, "cache.json"));
        public static readonly string CustomDataFilePath = new(Path.Join(AppDataPath, "custom_data.json"));

        public static readonly string[] BroadcasterScopes = ["channel:read:vips", "channel:read:subscriptions", "moderation:read"];
        public static readonly string[] ChatterScopes = ["user:write:chat", "user:read:chat"];

        public const string AllowedStateChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";

        public const int AuthorizationPortMin = 1024;
        public const int AuthorizationPortMax = 49151;
        public const int AuthorizationPortDefault = 27043;
        public const int NumMaxLogFilesMin = 1;
        public const int NumMaxLogFilesMax = 300;
        public const int NumMaxLogFilesDefault = 5;
        public const int WebSocketConnectTimeoutSecondsMin = 1;
        public const int WebSocketConnectTimeoutSecondsMax = 60;
        public const int WebSocketConnectTimeoutSecondsDefault = 5;
        public const int WebSocketKeepaliveTimeoutSecondsMin = 10;
        public const int WebSocketKeepaliveTimeoutSecondsMax = 600;
        public const int WebSocketKeepaliveTimeoutSecondsDefault = 10;
        public const int WebSocketKeepaliveTimeoutMarginSecondsMin = 1;
        public const int WebSocketKeepaliveTimeoutMarginSecondsMax = 60;
        public const int WebSocketKeepaliveTimeoutMarginSecondsDefault = 5;

        public const int AccessTokenExpirationMarginMillis = 300;
        public const int WebSocketClientDisconnectTimeoutSeconds = 3;
        public const int WebSocketClientFireCloseTimeoutSeconds = 3;
        public const int SendChatMessageFromScriptTimeoutSeconds = 3;
    }
}
