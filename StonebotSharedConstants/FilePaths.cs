namespace StonebotSharedConstants {
    using System;

    public static class FilePaths {
        public static readonly string AppDataDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string StonebotDataDirPath = Path.Join(AppDataDirPath, "Stonebot");
    }
}
