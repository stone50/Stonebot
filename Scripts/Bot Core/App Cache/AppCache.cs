namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Http;
    using System;
    using System.Threading.Tasks;

    internal static class AppCache {
        public class CacheValue<T> where T : class {
            private T? Value;
            private readonly Func<Task<T?>> ValueGetter;

            public CacheValue(T? value = default) : this(() => default(T?), value) { }

            public CacheValue(Func<T?> getter, T? value = default) : this(
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
        }

        public static CacheValue<Config> Config = new(App_Cache.Config.Create, null);
        public static CacheValue<AccessToken> AccessToken = new(App_Cache.AccessToken.Create, null);
        public static CacheValue<HttpClientWrapper> HttpClientWrapper = new(App_Cache.HttpClientWrapper.Create, null);
        public static CacheValue<Broadcaster> Broadcaster = new(App_Cache.Broadcaster.Create, null);
        public static CacheValue<EventSubWebSocketClient> EventSubWebSocketClient = new(App_Cache.EventSubWebSocketClient.Create, null);

        // TODO: store some cached values to disk on close, and load them on startup
    }
}
