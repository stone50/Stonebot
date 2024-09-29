namespace Stonebot.Scripts {
    using System;
    using System.IO;

    internal static class Constants {
        public static readonly string AppDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot");
        public static readonly string ConfigFilePath = Path.Join(AppDataPath, "config.json");
        public static readonly string CacheFilePath = Path.Join(AppDataPath, "cache.json");
        public static readonly string DataFilePath = Path.Join(AppDataPath, "data.json");
    }
}
