namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class AppCache {
        public class CacheValue<T>(Func<Task<T?>> getter, T? value = null) where T : class {
            public CacheValue(T? value = default) : this(() => default(T?), value) { }

            public CacheValue(Func<T?> getter, T? value = null) : this(
                async () => {
                    await Task.Yield();
                    return getter();
                },
                value
            ) { }

            public async Task<T?> Get() => value is null && !await Refresh() ? null : value;

            public async Task<bool> Refresh() {
                value = await getter();
                return value is not null;
            }

            public T? GetWithoutRefresh() => value;
        }

        public static string? StoredChatterRefreshToken => storedData?.ChatterRefreshToken;
        public static string? StoredCollectorRefreshToken => storedData?.CollectorRefreshToken;
        public static readonly CacheValue<Config> Config = new(App_Cache.Config.Create, null);
        public static readonly CacheValue<HttpClientWrapper> ChatterClientWrapper = new(HttpClientWrapper.CreateChatter, null);
        public static readonly CacheValue<HttpClientWrapper> CollectorClientWrapper = new(HttpClientWrapper.CreateCollector, null);
        public static readonly CacheValue<User> Bot = new(User.CreateBot, null);
        public static readonly CacheValue<User> Broadcaster = new(User.CreateBroadcaster, null);
        public static readonly CacheValue<WebSocketClient> WebSocketClient = new(() => new(), null);
        public static readonly CacheValue<CustomData> Data = new(CustomData.Create, null);

        public static async Task<bool> Init() {
            Logger.Info("Initializing app cache.");

            try {
                _ = Directory.CreateDirectory(Constants.AppDataPath);
            } catch (Exception e) {
                Logger.Warning($"Could not initialize app cache because directory create directory attempt failed: {e}.");
                return false;
            }

            if (File.Exists(Constants.CacheFilePath) && !await Load()) {
                Logger.Warning("App cache load attempt failed.");
            }

            return true;
        }

        public static async Task<bool> Load() {
            Logger.Info("Loading app cache.");

            string json;
            try {
                json = await File.ReadAllTextAsync(Constants.CacheFilePath);
            } catch (Exception e) {
                Logger.Warning($"Could not load app cache because file read all text attempt failed: {e}.");
                return false;
            }

            try {
                storedData = JsonSerializer.Deserialize<AppCacheData>(json);
            } catch (Exception e) {
                Logger.Warning($"Could not load app cache because json serializer deserialize attempt failed: {e}.");
                return false;
            }

            return true;
        }

        public static async Task<bool> SaveCache() {
            Logger.Info("Saving app cache.");

            var chatterClientWrapper = await ChatterClientWrapper.Get();
            if (chatterClientWrapper is null) {
                Logger.Warning("Could not save app cache because chatter client wrapper get attempt failed.");
                return false;
            }

            var collectorClientWrapper = await CollectorClientWrapper.Get();
            if (collectorClientWrapper is null) {
                Logger.Warning("Could not save app cache because collector client wrapper get attept failed.");
                return false;
            }

            var data = new AppCacheData() {
                ChatterRefreshToken = chatterClientWrapper.RefreshToken,
                CollectorRefreshToken = collectorClientWrapper.RefreshToken,
            };
            try {
                await File.WriteAllTextAsync(Constants.CacheFilePath, JsonSerializer.Serialize(data));
            } catch (Exception e) {
                Logger.Warning($"Could not save app cache because file write all text attempt failed: {e}.");
                return false;
            }

            storedData = data;
            return true;
        }

        public static async Task<bool> SaveCustomData() {
            Logger.Info("Saving custom data.");

            var data = await Data.Get();
            if (data is null) {
                Logger.Warning("Could not save custom data because data get attempt failed.");
                return false;
            }

            try {
                await File.WriteAllTextAsync(Constants.DataFilePath, JsonSerializer.Serialize(data.ToDataData()));
            } catch (Exception e) {
                Logger.Warning($"Could not save custom data because file write all text attempt failed: {e}.");
                return false;
            }

            return true;
        }

        public static async Task<bool> SaveAll() {
            Logger.Info("Saving all app cache.");

            var success = true;
            if (!await SaveCustomData()) {
                Logger.Warning("Could not save all app cache because save custom data attempt failed.");
                success = false;
            }

            if (!await SaveCache()) {
                Logger.Warning("Could not save all app cache because save cache attempt failed.");
                success = false;
            }

            return success;
        }

        private static AppCacheData? storedData;
    }
}
