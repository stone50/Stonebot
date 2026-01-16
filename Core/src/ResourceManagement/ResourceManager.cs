namespace StonebotCore.ResourceManagement {
    using StonebotCore.Models;
    using StonebotSharedConstants;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ResourceManager {
        private static readonly string _configFilePath = Path.Join(FilePaths.StonebotDataDirPath, "config.json");
        private static readonly string _twitchRefreshTokenFilePath = Path.Join(FilePaths.StonebotDataDirPath, "twitch_refresh.token");
        private static readonly string _twitchClientSecretFilePath = Path.Join(FilePaths.StonebotDataDirPath, "twitch_client.secret");
        private static readonly string _customDataDirPath = Path.Join(FilePaths.StonebotDataDirPath, "custom data");
        private static readonly string _quotesFilePath = Path.Join(_customDataDirPath, "quotes.json");
        private static readonly string _feedDataFilePath = Path.Join(_customDataDirPath, "feed_data.json");
        private static readonly DataProtectionFileStore _protectedStore = new(FilePaths.StonebotDataDirPath);

        static ResourceManager() => Directory.CreateDirectory(_customDataDirPath);

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
                Access.Config = new();
                return;
            }

            var json = await File.ReadAllTextAsync(_configFilePath, cancellationToken).ConfigureAwait(false);
            Access.Config = JsonSerializer.Deserialize<Config>(json) ?? new();
        }

        internal static Task SaveConfigAsync(CancellationToken cancellationToken) {
            var json = JsonSerializer.Serialize(Access.Config);
            return File.WriteAllTextAsync(_configFilePath, json, cancellationToken);
        }

        internal static async Task<Quote[]> LoadQuotesAsync(CancellationToken cancellationToken) {
            if (!File.Exists(_quotesFilePath)) {
                return [];
            }

            var json = await File.ReadAllTextAsync(_quotesFilePath, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<Quote[]>(json) ?? [];
        }

        internal static Task SaveQuotesAsync(Quote[] quotes, CancellationToken cancellationToken) {
            var json = JsonSerializer.Serialize(quotes);
            return File.WriteAllTextAsync(_quotesFilePath, json, cancellationToken);
        }

        internal static async Task<FeedData> LoadFeedDataAsync(CancellationToken cancellationToken) {
            if (!File.Exists(_feedDataFilePath)) {
                return new();
            }

            var json = await File.ReadAllTextAsync(_feedDataFilePath, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FeedData>(json) ?? new();
        }

        internal static Task SaveFeedDataAsync(FeedData data, CancellationToken cancellationToken) {
            var json = JsonSerializer.Serialize(data);
            return File.WriteAllTextAsync(_feedDataFilePath, json, cancellationToken);
        }
    }
}
