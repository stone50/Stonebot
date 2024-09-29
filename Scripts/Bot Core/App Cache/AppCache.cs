namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class AppCache {
        public class CacheValue<T> where T : class {
            public CacheValue(T? value = default) : this(() => default(T?), value) { }

            public CacheValue(Func<T?> getter, T? value = null) : this(
                async () => {
                    await Task.Yield();
                    return getter();
                },
                value
            ) { }

            public CacheValue(Func<Task<T?>> getter, T? value = null) {
                ValueGetter = getter;
                Value = value;
            }

            public async Task<T?> Get() {
                Value ??= await ValueGetter();
                return Value;
            }

            public async Task Refresh() => Value = await ValueGetter();

            public T? GetWithoutRefresh() => Value;

            private T? Value;
            private readonly Func<Task<T?>> ValueGetter;
        }

        public static string? StoredChatterRefreshToken => storedData?.ChatterRefreshToken;
        public static string? StoredCollectorRefreshToken => storedData?.CollectorRefreshToken;
        public static CacheValue<Config> Config = new(App_Cache.Config.Create, null);
        public static CacheValue<HttpClientWrapper> ChatterClientWrapper = new(HttpClientWrapper.CreateChatter, null);
        public static CacheValue<HttpClientWrapper> CollectorClientWrapper = new(HttpClientWrapper.CreateCollector, null);
        public static CacheValue<User> Bot = new(User.CreateBot, null);
        public static CacheValue<User> Broadcaster = new(User.CreateBroadcaster, null);
        public static CacheValue<WebSocketClient> WebSocketClient = new(() => new(), null);
        public static CacheValue<CustomData> Data = new(CustomData.Create, null);

        public static async Task<bool> Init() {
            Logger.Info("Initializing app cache.");
            _ = await Load();
            var success = await CollectorClientWrapper.Get() is not null;
            return await ChatterClientWrapper.Get() is not null && success;
        }

        public static async Task<bool> Load() {
            Logger.Info("Loading app cache.");
            string json;
            try {
                json = await File.ReadAllTextAsync("cache.json");
            } catch {
                return false;
            }

            try {
                storedData = JsonSerializer.Deserialize<AppCacheData>(json);
            } catch {
                return false;
            }

            return true;
        }

        public static async Task<bool> SaveCache() {
            Logger.Info("Saving cache to file.");
            var chatterClientWrapper = await ChatterClientWrapper.Get();
            if (chatterClientWrapper is null) {
                return false;
            }

            var collectorClientWrapper = await CollectorClientWrapper.Get();
            if (collectorClientWrapper is null) {
                return false;
            }

            var data = new AppCacheData() {
                ChatterRefreshToken = chatterClientWrapper.RefreshToken,
                CollectorRefreshToken = collectorClientWrapper.RefreshToken,
            };
            try {
                await File.WriteAllTextAsync("cache.json", JsonSerializer.Serialize(data));
            } catch (Exception e) {
                Logger.Warning($"Cannot save cache because File.WriteAllTextAsync failed: {e}.");
                return false;
            }

            storedData = data;
            return true;
        }

        public static async Task<bool> SaveCustomData() {
            Logger.Info("Saving custom data to file.");
            var data = await Data.Get();
            if (data is null) {
                return false;
            }

            try {
                await File.WriteAllTextAsync("data.json", JsonSerializer.Serialize(data.ToDataData()));
            } catch (Exception e) {
                Logger.Warning($"Cannot save custom data because File.WriteAllTextAsync failed: {e}.");
                return false;
            }

            return true;
        }

        public static async Task Save() {
            Logger.Info("Saving app cache.");
            _ = await SaveCustomData();
            _ = await SaveCache();
        }

        private static AppCacheData? storedData;
    }
}
