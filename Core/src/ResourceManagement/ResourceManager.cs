namespace StonebotCore.ResourceManagement {
    using StonebotSharedConstants;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ResourceManager {
        private static readonly string _configFilePath = Path.Join(FilePaths.StonebotDataDirPath, "config.json");
        private static readonly string _twitchRefreshTokenFilePath = Path.Join(FilePaths.StonebotDataDirPath, "twitch_refresh.token");
        private static readonly string _twitchClientSecretFilePath = Path.Join(FilePaths.StonebotDataDirPath, "twitch_client.secret");
        private static readonly DataProtectionFileStore _protectedStore = new(FilePaths.StonebotDataDirPath);

        static ResourceManager() => Directory.CreateDirectory(FilePaths.StonebotDataDirPath);

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
