namespace Stonebot.Scripts {
    using Godot;
    using System.IO;

    internal static class Constants {
        public static readonly string AppDataPath = ProjectSettings.GlobalizePath("user://");
        public static readonly string ConfigFilePath = Path.Join(AppDataPath, "config.json");
        public static readonly string CacheFilePath = Path.Join(AppDataPath, "cache.json");
        public static readonly string DataFilePath = Path.Join(AppDataPath, "data.json");
    }
}
