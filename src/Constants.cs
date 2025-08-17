namespace Stonebot {
    using System.IO;

    internal static class Constants {
        // Avalonia resource paths
        public const string BaseAvaloniaResourcePath = "avares://Stonebot/Resources";
        public static readonly string LogoAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "logo.png");
        public static readonly string CogAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "cog.png");
        public static readonly string PowerAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "power.png");
        public static readonly string CheckAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "check.png");
        public static readonly string CrossAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "cross.png");
        public static readonly string PencilAvaloniaResourceFilePath = Path.Join(BaseAvaloniaResourcePath, "pencil.png");

        // Stonebot config paths
        public static readonly string LocalAppDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stonebot");
        public static readonly string LogsPath = Path.Join(LocalAppDataPath, "logs");
        public static readonly string ConfigFilePath = Path.Join(LocalAppDataPath, "config.bin");
        public static readonly string CacheFilePath = Path.Join(LocalAppDataPath, "cache.bin");
        public static readonly string CustomDataFilePath = Path.Join(LocalAppDataPath, "custom_data.json");
        public static readonly string CommandManagerFilePath = Path.Join(LocalAppDataPath, "command_manager.bin");

        // scripting paths
        public static readonly string ScriptsPath = Path.Join(LocalAppDataPath, "scripts");
        public static readonly string ScriptsTypeHintsPackagePath = Path.Join(ScriptsPath, "stonebot");
        public static readonly string ScriptsTypeHintsFilePath = Path.Join(ScriptsTypeHintsPackagePath, "__init__.pyi");
        public static readonly string CommandScriptsPath = Path.Join(ScriptsPath, "commands");

        // authorization
        public const string AuthRedirectUri = "http://localhost:27043/botauth/";
        public const string AuthScopes = "user:write:chat user:read:chat";
        public const string AuthStateAllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";
        public const int AuthStateLength = 32;

        // command limits
        public const int CommandCooldownSecsMax = int.MaxValue;
        public const int NumMaxCommandNameChars = 15;

        // config limits
        public const int NumMaxLogFilesMin = 1;
        public const int NumMaxLogFilesMax = 300;
        public const int NumMaxLogFilesDefault = 5;

        public const int DefaultCancellationTokenTimeoutSecs = 10;

        public const int AccessTokenExpirationMarginSecs = 10;

        public const int WebSocketKeepaliveTimeoutSecs = 10;
        public const int WebSocketKeepaliveTimeoutMarginSecs = 10;
        public const int WebSocketRequestBufferLength = 65536;
    }
}
