namespace Stonebot {
    using Models.Data;
    using System.Text.Json;

    internal static class Cache {
        public static readonly HttpClient DefaultClient = new();
        public static AuthorizationData? BroadcasterAuthorizationData { get; private set; }
        public static AuthorizationData? ChatterAuthorizationData { get; private set; }

        public static void Init(CancellationToken cancellationToken) {
            if (!File.Exists(Constants.CacheFilePath)) {
                return;
            }

            var cacheFileContents = File.ReadAllText(Constants.CacheFilePath);
            var cacheData = JsonSerializer.Deserialize(cacheFileContents, JsonContext.Default.CacheData);
            if (cacheData.BroadcasterRefreshToken is not null) {
                BroadcasterAuthorizationData = AuthorizationData.Create(Config.BroadcasterClientId, Config.BroadcasterClientSecret, cacheData.BroadcasterRefreshToken, cancellationToken);
            }

            if (cacheData.ChatterRefreshToken is not null) {
                ChatterAuthorizationData = AuthorizationData.Create(Config.ChatterClientId, Config.ChatterClientSecret, cacheData.ChatterRefreshToken, cancellationToken);
            }
        }

        public static void CreateBroadcasterAuthorizationData(CancellationToken cancellationToken) {
            ClearBroadcasterAuthorizationData();
            BroadcasterAuthorizationData = AuthorizationData.Create(Config.BroadcasterClientId, Config.BroadcasterClientSecret, Constants.BroadcasterScopes, cancellationToken);
        }

        public static void CreateChatterAccessToken(CancellationToken cancellationToken) {
            ClearChatterAuthorizationData();
            ChatterAuthorizationData = AuthorizationData.Create(Config.ChatterClientId, Config.ChatterClientSecret, Constants.ChatterScopes, cancellationToken);
        }

        public static void ClearBroadcasterAuthorizationData() {
            BroadcasterAuthorizationData?.Dispose();
            BroadcasterAuthorizationData = null;
        }

        public static void ClearChatterAuthorizationData() {
            ChatterAuthorizationData?.Dispose();
            ChatterAuthorizationData = null;
        }

        public static void Save() {
            var contents = JsonSerializer.Serialize(new CacheData() {
                BroadcasterRefreshToken = BroadcasterAuthorizationData?.AccessToken.RefreshToken,
                ChatterRefreshToken = ChatterAuthorizationData?.AccessToken.RefreshToken,
            }, JsonContext.Default.CacheData);
            File.WriteAllText(Constants.CacheFilePath, contents);
        }
    }
}
