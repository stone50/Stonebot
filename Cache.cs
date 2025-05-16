namespace Stonebot {
    using Models;
    using System.Text.Json;

    internal static class Cache {
        public static readonly HttpClient DefaultClient = new();
        public static AuthorizationData? BroadcasterAuthorizationData { get; private set; }
        public static AuthorizationData? ChatterAuthorizationData { get; private set; }

        public static async Task InitAsync(CancellationToken cancellationToken) {
            if (!File.Exists(Constants.CacheFilePath)) {
                return;
            }

            try {
                var cacheFileContents = await File.ReadAllTextAsync(Constants.CacheFilePath, cancellationToken).ConfigureAwait(false);
                var cacheData = JsonSerializer.Deserialize(cacheFileContents, JsonContext.Default.CacheData);
                if (cacheData.BroadcasterRefreshToken is not null) {
                    BroadcasterAuthorizationData = await AuthorizationData.CreateAsync(Config.BroadcasterClientId, Config.BroadcasterClientSecret, cacheData.BroadcasterRefreshToken, cancellationToken).ConfigureAwait(false);
                }

                if (cacheData.ChatterRefreshToken is not null) {
                    ChatterAuthorizationData = await AuthorizationData.CreateAsync(Config.ChatterClientId, Config.ChatterClientSecret, cacheData.ChatterRefreshToken, cancellationToken).ConfigureAwait(false);
                }
            } catch (OperationCanceledException) {
                return;
            } catch (Exception e) {
                Logger.Warn(e);
            }
        }

        public static async Task CreateBroadcasterAuthorizationDataAsync(CancellationToken cancellationToken) {
            ClearBroadcasterAuthorizationData();
            BroadcasterAuthorizationData = await AuthorizationData.CreateAsync(Config.BroadcasterClientId, Config.BroadcasterClientSecret, Config.BroadcasterScopes, cancellationToken).ConfigureAwait(false);
        }

        public static async Task CreateChatterAccessTokenAsync(CancellationToken cancellationToken) {
            ClearChatterAuthorizationData();
            ChatterAuthorizationData = await AuthorizationData.CreateAsync(Config.ChatterClientId, Config.ChatterClientSecret, Config.ChatterScopes, cancellationToken).ConfigureAwait(false);
        }

        public static void ClearBroadcasterAuthorizationData() {
            BroadcasterAuthorizationData?.Dispose();
            BroadcasterAuthorizationData = null;
        }

        public static void ClearChatterAuthorizationData() {
            ChatterAuthorizationData?.Dispose();
            ChatterAuthorizationData = null;
        }

        public static void ClearAll() {
            ClearBroadcasterAuthorizationData();
            ClearChatterAuthorizationData();
        }

        public static Task SaveAsync(CancellationToken cancellationToken) {
            var contents = JsonSerializer.Serialize(new CacheData() {
                BroadcasterRefreshToken = BroadcasterAuthorizationData?.AccessToken.RefreshToken,
                ChatterRefreshToken = ChatterAuthorizationData?.AccessToken.RefreshToken,
            }, JsonContext.Default.CacheData);
            return File.WriteAllTextAsync(Constants.CacheFilePath, contents, cancellationToken);
        }
    }
}
