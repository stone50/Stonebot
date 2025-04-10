namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Twitch;

    internal class AccessToken {
        public readonly string ClientId;
        public readonly string ClientSecret;
        public string RefreshToken { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public int ExpirationBuffer { get; private set; }

        public bool IsAboutToExpire => DateTime.Now.AddMilliseconds(ExpirationBuffer) >= ExpirationDate;

        public static async Task<AccessToken?> CreateChatter() {
            var logPrefix = $"{nameof(AccessToken)} | {nameof(CreateChatter)}";
            Logger.Info(logPrefix);

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return null;
            }

            var createdChatter = await Create(config.ChatterClientId, config.ChatterClientSecret, AppCache.StoredChatterRefreshToken, config.ChatterScope, config.TokenExpirationBuffer);
            if (createdChatter is null) {
                Logger.Warning($"{logPrefix} | {nameof(Create)} result is null.");
                return null;
            }

            return createdChatter;
        }

        public static async Task<AccessToken?> CreateCollector() {
            var logPrefix = $"{nameof(AccessToken)} | {nameof(CreateCollector)}";
            Logger.Info(logPrefix);

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return null;
            }

            var createdCollector = await Create(config.CollectorClientId, config.CollectorClientSecret, AppCache.StoredCollectorRefreshToken, config.CollectorScope, config.TokenExpirationBuffer);
            if (createdCollector is null) {
                Logger.Warning($"{logPrefix} | {nameof(Create)} result is null.");
                return null;
            }

            return createdCollector;

        }

        public async Task<string?> GetString() {
            var logPrefix = $"{nameof(AccessToken)} | {nameof(GetString)}";
            Logger.Info(logPrefix);

            if (!IsAboutToExpire) {
                return accessToken;
            }

            if (!await Refresh()) {
                Logger.Warning($"{logPrefix} | {nameof(Refresh)} result is false.");
                return null;
            }

            return accessToken;
        }

        public async Task<bool> Refresh() {
            var logPrefix = $"{nameof(AccessToken)} | {nameof(Refresh)}";
            Logger.Info(logPrefix);

            var potentialData = await RequestRefresh(ClientId, ClientSecret, RefreshToken);
            if (potentialData is null) {
                Logger.Warning($"{logPrefix} | {nameof(RequestRefresh)} result is null.");
                return false;
            }

            var data = (AccessTokenData)potentialData;
            ExpirationDate = DateTime.Now.AddSeconds(data.ExpiresIn);
            accessToken = data.AccessToken;
            RefreshToken = data.RefreshToken;
            return true;
        }

        public bool SetExpirationBuffer(int newExpirationBuffer) {
            var logPrefix = $"{nameof(AccessToken)} | {nameof(SetExpirationBuffer)}";
            Logger.Info($"{logPrefix}\n{nameof(newExpirationBuffer)}: {newExpirationBuffer}");

            if (newExpirationBuffer < 0) {
                Logger.Warning($"{logPrefix} | {nameof(newExpirationBuffer)} is < 0.");
                return false;
            }

            ExpirationBuffer = newExpirationBuffer;
            return true;
        }

        private string accessToken;

        private AccessToken(string clientId, string clientSecret, AccessTokenData data, int expirationBuffer) {
            var logPrefix = $"{nameof(AccessToken)} | Constructor";
            Logger.Info($"{logPrefix}\n{nameof(clientId)}: {clientId}\n{nameof(clientSecret)}: {clientSecret}\n{nameof(data)}: {JsonSerializer.Serialize(data)}\n{nameof(expirationBuffer)}: {expirationBuffer}");

            if (expirationBuffer < 0) {
                Logger.Error($"{logPrefix} | {nameof(expirationBuffer)} is < 0.");
                throw new ArgumentOutOfRangeException(nameof(expirationBuffer));
            }

            ClientId = clientId;
            ClientSecret = clientSecret;
            accessToken = data.AccessToken;
            RefreshToken = data.RefreshToken;
            ExpirationDate = DateTime.Now.AddSeconds(data.ExpiresIn);
            ExpirationBuffer = expirationBuffer;
        }

        private static async Task<AccessToken?> Create(string clientId, string clientSecret, string? storedRefreshToken, string[] scope, int expirationBuffer) {
            var logPrefix = $"{nameof(AccessToken)} | {nameof(Create)}";
            Logger.Info($"{logPrefix}\n{nameof(clientId)}: {clientId}\n{nameof(clientSecret)}: {clientSecret}\n{nameof(storedRefreshToken)}: {storedRefreshToken}\n{nameof(scope)}: {JsonSerializer.Serialize(scope)}\n{nameof(expirationBuffer)}: {expirationBuffer}");

            if (storedRefreshToken is not null) {
                var potentialRefreshData = await RequestRefresh(clientId, clientSecret, storedRefreshToken);
                if (potentialRefreshData is not null) {
                    AccessToken refreshedAccessToken;
                    try {
                        refreshedAccessToken = new(clientId, clientSecret, (AccessTokenData)potentialRefreshData, expirationBuffer);
                    } catch (Exception e) {
                        Logger.Warning($"{logPrefix} | Constructor threw: {e}.");
                        return null;
                    }

                    return refreshedAccessToken;
                }
            }

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Config.Get)} result is null.");
                return null;
            }

            var code = await AuthorizationCode.Create(clientId, scope);
            if (code is null) {
                Logger.Warning($"{logPrefix} | {nameof(AuthorizationCode.Create)} result is null.");
                return null;
            }

            var potentialData = await Util.GetMessageAs<AccessTokenData>(TwitchAPI.GetAccessToken(
                 new(),
                 clientId,
                 clientSecret,
                 code,
                 $"http://localhost:{config.AuthorizationPort}"
            ));
            if (potentialData is null) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.GetAccessToken)} was not successful.");
                return null;
            }

            AccessToken createdAccessToken;
            try {
                createdAccessToken = new(clientId, clientSecret, (AccessTokenData)potentialData, expirationBuffer);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix}| Constructor threw: {e}.");
                return null;
            }

            return createdAccessToken;
        }

        private static async Task<AccessTokenData?> RequestRefresh(string clientId, string clientSecret, string refreshToken) {
            var logPrefix = $"{nameof(AccessToken)} | {nameof(RequestRefresh)}";
            Logger.Info($"{logPrefix}\n{nameof(clientId)}: {clientId}\n{nameof(clientSecret)}: {clientSecret}\n{nameof(refreshToken)}: {refreshToken}");

            var accessTokenData = await Util.GetMessageAs<AccessTokenData>(TwitchAPI.RefreshAccessToken(
                new(),
                clientId,
                clientSecret,
                refreshToken
            ));
            if (accessTokenData is null) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.RefreshAccessToken)} was not successful.");
                return null;
            }

            return accessTokenData;
        }
    }
}
