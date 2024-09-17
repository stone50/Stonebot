namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using StoneBot.Scripts.Bot_Core.Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class AppCache {
        public class CacheValue<T> where T : class {
            private T? Value;
            private readonly Func<Task<T?>> ValueGetter;

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
        }

        public static string? StoredRefreshToken => storedData?.RefreshToken;
        public static CacheValue<Config> Config = new(App_Cache.Config.Create, null);
        public static CacheValue<AccessToken> AccessToken = new(App_Cache.AccessToken.Create, null);
        public static CacheValue<HttpClientWrapper> HttpClientWrapper = new(App_Cache.HttpClientWrapper.Create, null);
        public static CacheValue<Broadcaster> Broadcaster = new(App_Cache.Broadcaster.Create, null);
        public static CacheValue<Bot> Bot = new(App_Cache.Bot.Create, null);
        public static CacheValue<WebSocketClient> WebSocketClient = new(() => new(), null);

        public static async Task<bool> Load() {
            var filePath = Path.Join(Directory.GetCurrentDirectory(), "cache.json");
            string json;
            try {
                json = await File.ReadAllTextAsync(filePath);
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

        public static async Task<bool> Save() {
            var refreshToken = AccessToken.GetWithoutRefresh()?.RefreshToken;
            if (refreshToken is null) {
                return false;
            }

            var data = new AppCacheData() {
                RefreshToken = refreshToken
            };
            var json = JsonSerializer.Serialize(data);
            var filePath = Path.Join(Directory.GetCurrentDirectory(), "cache.json");
            try {
                await File.WriteAllTextAsync(filePath, json);
            } catch (Exception e) {
                GD.PushWarning($"Cannot save because File.WriteAllTextAsync failed: {e}.");
                return false;
            }

            storedData = data;
            return true;
        }

        private static AppCacheData? storedData;
    }
}
