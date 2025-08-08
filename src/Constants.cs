namespace Stonebot {
    using System.IO;

    internal static class Constants {
        public const string BaseAvaloniaResourcePath = "avares://Stonebot/Resources";
        public static readonly string LogoAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "logo.png");
        public static readonly string CogAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "cog.png");
        public static readonly string PowerAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "power.png");

        public static readonly string LocalAppDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stonebot");
        public static readonly string LogsPath = Path.Join(LocalAppDataPath, "logs");
        public static readonly string ConfigFilePath = Path.Join(LocalAppDataPath, "config.json");
        public static readonly string CacheFilePath = Path.Join(LocalAppDataPath, "cache.json");
        public static readonly string CustomDataFilePath = Path.Join(LocalAppDataPath, "custom_data.json");
        public static readonly string CommandManagerFilePath = Path.Join(LocalAppDataPath, "command_manager.json");
        public static readonly string ScriptsPath = Path.Join(LocalAppDataPath, "scripts");
        public static readonly string ScriptsTypeHintsPackagePath = Path.Join(ScriptsPath, "stonebot");
        public static readonly string ScriptsTypeHintsFilePath = Path.Join(ScriptsTypeHintsPackagePath, "__init__.pyi");
        public static readonly string CommandScriptsPath = Path.Join(ScriptsPath, "commands");

        public const string BroadcasterScopes = "channel:read:vips channel:read:subscriptions moderation:read";
        public const string ChatterScopes = "user:write:chat user:read:chat";

        public const string AllowedStateChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";

        public const int AuthorizationPortMin = 1024;
        public const int AuthorizationPortMax = 49151;
        public const int AuthorizationPortDefault = 27043;
        public const int CommandCooldownSecondsMax = int.MaxValue;
        public const int NumMaxCommandNameChars = 15;
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
        public const int GetHighestUserPermissionLevelTimeoutSeconds = 5;
        public const int SendChatMessageFromScriptTimeoutSeconds = 3;
        public const int WebSocketClientDisconnectTimeoutSeconds = 3;
        public const int WebSocketClientFireCloseTimeoutSeconds = 3;
    }
}
