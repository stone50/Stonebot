namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.Threading.Tasks;
    using Twitch;
    using HttpClient = System.Net.Http.HttpClient;

    internal class AccessToken {
        public readonly string ClientId;
        public readonly string ClientSecret;
        public string RefreshToken { get; private set; }
        public DateTime ExpirationDate { get; protected set; }
        public int ExpirationBuffer { get => expirationBuffer; set => SetExpirationBuffer(value); }

        public bool IsAboutToExpire => DateTime.Now.AddMilliseconds(ExpirationBuffer) >= ExpirationDate;

        public static async Task<AccessToken?> CreateChatter() {
            Logger.Info("Creating chatter access token.");
            var config = await AppCache.Config.Get();
            return config is null ? null : await Create(config.ChatterClientId, config.ChatterClientSecret, AppCache.StoredChatterRefreshToken, config.ChatterScope);
        }

        public static async Task<AccessToken?> CreateCollector() {
            Logger.Info("Creating collector access token.");
            var config = await AppCache.Config.Get();
            return config is null ? null : await Create(config.CollectorClientId, config.CollectorClientSecret, AppCache.StoredCollectorRefreshToken, config.CollectorScope);

        }

        public async Task<string?> GetString() => IsAboutToExpire && !await Refresh() ? null : accessToken;

        public async Task<bool> Refresh() {
            Logger.Info("Refreshing access token.");
            var potentialData = await Refresh(ClientId, ClientSecret, RefreshToken);
            if (potentialData is null) {
                return false;
            }

            var data = (AccessTokenData)potentialData;
            ExpirationDate = DateTime.Now.AddSeconds(data.ExpiresIn);
            accessToken = data.AccessToken;
            RefreshToken = data.RefreshToken;
            return true;
        }

        private string accessToken;
        private int expirationBuffer = 1000;

        private AccessToken(string clientId, string clientSecret, AccessTokenData data) {
            ClientId = clientId;
            ClientSecret = clientSecret;
            accessToken = data.AccessToken;
            RefreshToken = data.RefreshToken;
            ExpirationDate = DateTime.Now.AddSeconds(data.ExpiresIn);
        }

        private static async Task<AccessToken?> Create(string clientId, string clientSecret, string? storedRefreshToken, string[] scope) {
            if (storedRefreshToken is not null) {
                var potentialRefreshData = await Refresh(clientId, clientSecret, storedRefreshToken);
                if (potentialRefreshData is not null) {
                    return new(clientId, clientSecret, (AccessTokenData)potentialRefreshData);
                }
            }

            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var code = await AuthorizationCode.Create(clientId, scope);
            if (code is null) {
                return null;
            }

            var potentialData = await Util.GetMessageAs<AccessTokenData>(TwitchAPI.GetAccessToken(
                 new HttpClient(),
                 clientId,
                 clientSecret,
                 code,
                 $"http://localhost:{config.AuthorizationPort}"
             ));
            return potentialData is null ? null : new(clientId, clientSecret, (AccessTokenData)potentialData);
        }

        private static async Task<AccessTokenData?> Refresh(
            string clientId,
            string clientSecret,
            string refreshToken
        ) => await Util.GetMessageAs<AccessTokenData>(TwitchAPI.RefreshAccessToken(
            new HttpClient(),
            clientId,
            clientSecret,
            refreshToken
        ));

        private void SetExpirationBuffer(int newExpirationBuffer) {
            Logger.Info("Setting access token expiration buffer.");
            if (newExpirationBuffer < 0) {
                Logger.Error("Cannot set expiration buffer because newExpirationBuffer is less than 0.");
                throw new ArgumentOutOfRangeException(nameof(newExpirationBuffer));
            }

            expirationBuffer = newExpirationBuffer;
        }
    }
}
