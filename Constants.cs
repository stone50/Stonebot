namespace Stonebot {
    using System.IO;

    internal static class Constants {
        [Flags]
        public enum ExitCode {
            OK = 0,
            LoggerInitError = 1 << 0,
            LoggerDeleteExcessFilesError = 1 << 1,
            LoggerShutdownError = 1 << 2,
        }

        public static readonly string AppDataPath = new(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot"));
        public static readonly string LogsPath = new(Path.Join(AppDataPath, "logs"));
        public static readonly string ConfigFilePath = new(Path.Join(AppDataPath, "config.json"));
        public static readonly string CacheFilePath = new(Path.Join(AppDataPath, "cache.json"));
        public static readonly string DataFilePath = new(Path.Join(AppDataPath, "data.json"));

        public static readonly string AllowedStateChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.-~";
    }
}
