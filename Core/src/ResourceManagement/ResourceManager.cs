namespace StonebotCore.ResourceManagement {
    using System;
    using System.IO;

    internal static class ResourceManager {
        internal static readonly string AppDataDirPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot");
        private static readonly string _twitchRefreshTokenFilePath = Path.Join(AppDataDirPath, "twitch_refresh.token");
        private static readonly IProtectedFileStore _protectedFileStore = ProtectedFileStore.Create();

        internal static string LoadTwitchRefreshToken() => _protectedFileStore.Load(_twitchRefreshTokenFilePath);

        internal static void SaveTwitchRefreshToken(string refreshToken) => _protectedFileStore.Save(refreshToken, _twitchRefreshTokenFilePath);
    }
}
