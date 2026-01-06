namespace StonebotCore.ResourceManagement {
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ResourceManager {
        private static readonly string _appDataDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string StonebotDataDirPath = Path.Combine(_appDataDirPath, "Stonebot");
        private static readonly string _configFilePath = Path.Combine(StonebotDataDirPath, "config.json");
        private static readonly string _twitchRefreshTokenFilePath = Path.Combine(StonebotDataDirPath, "twitch_refresh.token");
        private static readonly string _twitchClientSecretFilePath = Path.Combine(StonebotDataDirPath, "twitch_client.secret");
        private static readonly IProtectedFileStore _protectedStore = ProtectedFileStore.Instance;

        static ResourceManager() => Directory.CreateDirectory(StonebotDataDirPath);

        internal static Task SaveTwitchRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken) =>
            _protectedStore.SaveAsync(_twitchRefreshTokenFilePath, refreshToken, cancellationToken);

        internal static Task<string> LoadTwitchRefreshTokenAsync(CancellationToken cancellationToken) =>
            _protectedStore.LoadAsync(_twitchRefreshTokenFilePath, cancellationToken);

        internal static Task SaveTwitchClientSecretAsync(string clientSecret, CancellationToken cancellationToken) =>
            _protectedStore.SaveAsync(_twitchClientSecretFilePath, clientSecret, cancellationToken);

        internal static Task<string> LoadTwitchClientSecretAsync(CancellationToken cancellationToken) =>
            _protectedStore.LoadAsync(_twitchClientSecretFilePath, cancellationToken);

        internal static async Task LoadConfigAsync(CancellationToken cancellationToken) {
            if (!File.Exists(_configFilePath)) {
                Access.Config = new Models.Config();
                return;
            }

            var json = await File.ReadAllTextAsync(_configFilePath, cancellationToken).ConfigureAwait(false);
            Access.Config = JsonSerializer.Deserialize<Models.Config>(json) ?? new Models.Config();
        }

        internal static Task SaveConfigAsync(CancellationToken cancellationToken) {
            var json = JsonSerializer.Serialize(Access.Config);
            return File.WriteAllTextAsync(_configFilePath, json, cancellationToken);
        }
    }
}
