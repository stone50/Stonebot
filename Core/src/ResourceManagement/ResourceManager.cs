namespace StonebotCore.ResourceManagement {
    using StonebotCore.Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ResourceManager {
        private static readonly string _appDataDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string StonebotDataDirPath = Path.Combine(_appDataDirPath, "Stonebot");
        private static readonly string _configFilePath = Path.Join(StonebotDataDirPath, "config.json");
        private static readonly string _twitchRefreshTokenFilePath = Path.Join(StonebotDataDirPath, "twitch_refresh.token");
        private static readonly string _twitchClientSecretFilePath = Path.Join(StonebotDataDirPath, "twitch_client.secret");
        private static readonly IProtectedFileStore _protectedFileStore = ProtectedFileStore.Create();

        internal static Task<string> LoadTwitchRefreshTokenAsync(
            CancellationToken cancellationToken
        ) => _protectedFileStore.LoadAsync(
            filePath: _twitchRefreshTokenFilePath,
            cancellationToken
        );

        internal static Task SaveTwitchRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken
        ) {
            _ = Directory.CreateDirectory(StonebotDataDirPath);
            return _protectedFileStore.SaveAsync(
                filePath: _twitchRefreshTokenFilePath,
                data: refreshToken,
                cancellationToken
            );
        }

        internal static Task<string> LoadTwitchClientSecretAsync(
            CancellationToken cancellationToken
        ) => _protectedFileStore.LoadAsync(
            filePath: _twitchClientSecretFilePath,
            cancellationToken
        );

        internal static Task SaveTwitchClientSecretAsync(
            string clientSecret,
            CancellationToken cancellationToken
        ) {
            _ = Directory.CreateDirectory(StonebotDataDirPath);
            return _protectedFileStore.SaveAsync(
                filePath: _twitchClientSecretFilePath,
                data: clientSecret,
                cancellationToken
            );
        }

        internal static async Task LoadConfigAsync(
            CancellationToken cancellationToken
        ) {
            var configJson = await File.ReadAllTextAsync(
                path: _configFilePath,
                cancellationToken
            );
            Access.Config = JsonSerializer.Deserialize<Config>(configJson)!;
        }

        internal static Task SaveConfigAsync(
            CancellationToken cancellationToken
        ) {
            _ = Directory.CreateDirectory(StonebotDataDirPath);
            var configJson = JsonSerializer.Serialize(Access.Config);
            return File.WriteAllTextAsync(
                path: _configFilePath,
                contents: configJson,
                cancellationToken
            );
        }
    }
}
