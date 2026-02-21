namespace StonebotDaemon.TwitchMessageHandling {
    using StonebotCore.Models;
    using StonebotDaemon.Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ResourceManager {
        private static readonly string _customDataDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot", "custom data");
        private static readonly string _quotesFilePath = Path.Combine(_customDataDirPath, "quotes.json");
        private static readonly string _feedDataFilePath = Path.Combine(_customDataDirPath, "feed_data.json");

        static ResourceManager() => Directory.CreateDirectory(_customDataDirPath);

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
