namespace StoneBot.Scripts.App_Cache {
    using Access_Token;
    using Http;
    using Http.Access_Token_Client;
    using Models;
    using System;
    using System.Threading.Tasks;
    using Value_Getters;

    internal static class AppCache {
        public class CacheValue<T> where T : struct {
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

            public T? GetWithoutReload() => Value;

            public void Clear() => Value = null;
        }

        public class CacheRValue<T> where T : class {
            private T? Value;
            private readonly Func<Task<T?>> ValueGetter;

            public CacheRValue(T? value = default) : this(() => default(T?), value) { }

            public CacheRValue(Func<T?> getter, T? value = default) : this(
                async () => {
                    await Task.Yield();
                    return getter();
                },
                value
            ) { }

            public CacheRValue(Func<Task<T?>> getter, T? value = null) {
                ValueGetter = getter;
                Value = value;
            }

            public async Task<T?> Get() {
                Value ??= await ValueGetter();
                return Value;
            }

            public T? GetWithoutReload() => Value;

            public void Clear() => Value = null;
        }

        public static CacheValue<ConfigValues> ConfigValues = new(ValueGetters.GetConfigValues, null);
        public static CacheRValue<string> AuthorizationCode = new(ValueGetters.GetAuthorizationCode, null);
        public static CacheRValue<AppAccessToken> AppAccessToken = new(ValueGetters.GetAppAccessToken, null);
        public static CacheRValue<UserAccessToken> UserAccessToken = new(ValueGetters.GetUserAccessToken, null);
        public static CacheRValue<HttpAppAccessTokenClient> HttpAppAccessTokenClient = new(ValueGetters.GetHttpAppAccessTokenClient, null);
        public static CacheRValue<HttpUserAccessTokenClient> HttpUserAccessTokenClient = new(ValueGetters.GetHttpUserAccessTokenClient, null);
        public static CacheRValue<string> BroadcasterId = new(ValueGetters.GetBroadcasterId, null);
        public static CacheRValue<EventSubWebSocketClient> EventSubWebSocketClient = new(ValueGetters.GetEventSubWebSocketClient, null);

        // TODO: store some cached values to disk on close, and load them on startup
    }
}
