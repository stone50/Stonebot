namespace Stonebot {
    using System.IO;

    internal static class Constants {
        public static readonly string AppDataPath = new(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot"));
        public static readonly string LogsPath = new(Path.Join(AppDataPath, "logs"));
        public static readonly string ConfigFilePath = new(Path.Join(AppDataPath, "config.json"));
        public static readonly string CacheFilePath = new(Path.Join(AppDataPath, "cache.json"));
        public static readonly string DataFilePath = new(Path.Join(AppDataPath, "data.json"));

        public static readonly string[] BroadcasterScopes = ["channel:read:vips", "channel:read:subscriptions", "moderation:read"];
        public static readonly string[] ChatterScopes = ["user:write:chat", "user:read:chat"];

        public const string AllowedStateChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";
        public const int WebSocketClientKeepaliveTimeoutSeconds = 10;
        public const int WebSocketClientCloseOnCancelTimeoutSeconds = 10;
        public const int WebSocketClientConnectTimeoutSeconds = 10;
        public const int WebSocketClientDisconnectTimeoutSeconds = 10;
        public const int WebSocketClientFireCloseTimeoutSeconds = 10;
        public const int AccessTokenExpirationMarginMillis = 300;
    }
}
