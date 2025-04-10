namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class AppCache {
        public class CacheValue<T> where T : class {
            public CacheValue(Func<T?> getter) {
                Logger.Info($"{nameof(AppCache)} | {nameof(CacheValue<T>)}\n{nameof(getter)}: {getter}");

                this.getter = getter;
            }

            public T? Get() {
                var logPrefix = $"{nameof(AppCache)} | {nameof(CacheValue<T>)} | {nameof(Get)}";
                Logger.Info(logPrefix);

                if (value is not null) {
                    return value;
                }

                if (!Refresh()) {
                    Logger.Warning($"{logPrefix} | {nameof(Refresh)} result is false.");
                    return null;
                }

                return value;
            }

            public bool Refresh() {
                Logger.Info($"{nameof(AppCache)} | {nameof(CacheValue<T>)} | {nameof(Refresh)}");

                value = getter();
                return value is not null;
            }

            public T? GetWithoutRefresh() {
                Logger.Info($"{nameof(AppCache)} | {nameof(CacheValue<T>)} | {nameof(GetWithoutRefresh)}");

                return value;
            }

            public void Clear() {
                Logger.Info($"{nameof(AppCache)} | {nameof(CacheValue<T>)} | {nameof(Clear)}");

                value = null;
            }

            private T? value;
            private readonly Func<T?> getter;
        }

        public class AsyncCacheValue<T> where T : class {
            public AsyncCacheValue(Func<Task<T?>> getter) {
                Logger.Info($"{nameof(AppCache)} | {nameof(AsyncCacheValue<T>)}\n{nameof(getter)}: {getter}");

                this.getter = getter;
            }

            public async Task<T?> Get() {
                var logPrefix = $"{nameof(AppCache)} | {nameof(AsyncCacheValue<T>)} | {nameof(Get)}";
                Logger.Info(logPrefix);

                if (value is not null) {
                    return value;
                }

                if (!await Refresh()) {
                    Logger.Warning($"{logPrefix} | {nameof(Refresh)} result is false.");
                    return null;
                }

                return value;
            }

            public async Task<bool> Refresh() {
                Logger.Info($"{nameof(AppCache)} | {nameof(AsyncCacheValue<T>)} | {nameof(Refresh)}");

                value = await getter();
                return value is not null;
            }

            public T? GetWithoutRefresh() {
                Logger.Info($"{nameof(AppCache)} | {nameof(AsyncCacheValue<T>)} | {nameof(GetWithoutRefresh)}");

                return value;
            }

            public void Clear() {
                Logger.Info($"{nameof(AppCache)} | {nameof(AsyncCacheValue<T>)} | {nameof(Clear)}");

                value = null;
            }

            private T? value;
            private readonly Func<Task<T?>> getter;
        }

        public static string? StoredChatterRefreshToken => storedData?.ChatterRefreshToken;
        public static string? StoredCollectorRefreshToken => storedData?.CollectorRefreshToken;
        public static readonly AsyncCacheValue<Config> Config = new(App_Cache.Config.Create);
        public static readonly AsyncCacheValue<HttpClientWrapper> ChatterClientWrapper = new(HttpClientWrapper.CreateChatter);
        public static readonly AsyncCacheValue<HttpClientWrapper> CollectorClientWrapper = new(HttpClientWrapper.CreateCollector);
        public static readonly AsyncCacheValue<User> Bot = new(User.CreateBot);
        public static readonly AsyncCacheValue<User> Broadcaster = new(User.CreateBroadcaster);
        public static readonly CacheValue<WebSocketClient> WebSocketClient = new(() => new());
        public static readonly AsyncCacheValue<CustomData> Data = new(CustomData.Create);

        public static async Task<bool> Init() {
            var logPrefix = $"{nameof(AppCache)} | {nameof(Init)}";
            Logger.Info(logPrefix);

            try {
                _ = Directory.CreateDirectory(Constants.AppDataPath);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(Directory.CreateDirectory)} threw: {e}.\n{nameof(Constants.AppDataPath)}: {Constants.AppDataPath}");
                return false;
            }

            if (!File.Exists(Constants.CacheFilePath)) {
                return true;
            }

            if (!await Load()) {
                Logger.Warning($"{logPrefix} | {nameof(Load)} result is false.");
                return true;
            }

            return true;
        }

        public static async Task<bool> Load() {
            var logPrefix = $"{nameof(AppCache)} | {nameof(Load)}";
            Logger.Info(logPrefix);

            string json;
            try {
                json = await File.ReadAllTextAsync(Constants.CacheFilePath);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(File.ReadAllTextAsync)} threw: {e}.\n{nameof(Constants.CacheFilePath)}: {Constants.CacheFilePath}");
                return false;
            }

            try {
                storedData = JsonSerializer.Deserialize<AppCacheData>(json);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(JsonSerializer.Deserialize)} threw: {e}.\n{nameof(json)}: {json}");
                return false;
            }

            return true;
        }

        public static async Task<bool> SaveCache() {
            var logPrefix = $"{nameof(AppCache)} | {nameof(SaveCache)}";
            Logger.Info(logPrefix);

            var chatterClientWrapper = await ChatterClientWrapper.Get();
            if (chatterClientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(ChatterClientWrapper.Get)} result is null.");
                return false;
            }

            var collectorClientWrapper = await CollectorClientWrapper.Get();
            if (collectorClientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(CollectorClientWrapper.Get)} result is null.");
                return false;
            }

            var data = new AppCacheData() {
                ChatterRefreshToken = chatterClientWrapper.RefreshToken,
                CollectorRefreshToken = collectorClientWrapper.RefreshToken,
            };
            var serializedData = JsonSerializer.Serialize(data);
            try {
                await File.WriteAllTextAsync(Constants.CacheFilePath, serializedData);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(File.WriteAllTextAsync)} threw: {e}.\n{nameof(Constants.CacheFilePath)}: {Constants.CacheFilePath}\n{nameof(serializedData)}: {serializedData}");
                return false;
            }

            storedData = data;
            return true;
        }

        public static async Task<bool> SaveCustomData() {
            var logPrefix = $"{nameof(AppCache)} | {nameof(SaveCustomData)}";
            Logger.Info(logPrefix);

            var data = await Data.Get();
            if (data is null) {
                Logger.Warning($"{logPrefix} | {nameof(Data.Get)} result is null.");
                return false;
            }

            var serializedDataData = JsonSerializer.Serialize(data.ToDataData());
            try {
                await File.WriteAllTextAsync(Constants.DataFilePath, serializedDataData);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(File.WriteAllTextAsync)} threw: {e}.\n{nameof(Constants.DataFilePath)}: {Constants.DataFilePath}\n{nameof(serializedDataData)}: {serializedDataData}");
                return false;
            }

            return true;
        }

        public static async Task<bool> SaveAll() {
            var logPrefix = $"{nameof(AppCache)} | {nameof(SaveAll)}";
            Logger.Info(logPrefix);

            var success = true;
            if (!await SaveCustomData()) {
                Logger.Warning($"{logPrefix} | {nameof(SaveCustomData)} result is false.");
                success = false;
            }

            if (!await SaveCache()) {
                Logger.Warning($"{logPrefix} | {nameof(SaveCache)} result is false.");
                success = false;
            }

            return success;
        }

        private static AppCacheData? storedData;
    }
}
