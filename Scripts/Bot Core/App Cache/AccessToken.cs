namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.Threading.Tasks;
    using Twitch;

    internal class AccessToken {
        public readonly string ClientId;
        public readonly string ClientSecret;
        public string RefreshToken { get; private set; }
        public DateTime ExpirationDate { get; protected set; }
        public int ExpirationBuffer { get; private set; }

        public bool IsAboutToExpire => DateTime.Now.AddMilliseconds(ExpirationBuffer) >= ExpirationDate;

        public static async Task<AccessToken?> CreateChatter() {
            Logger.Info("Creating chatter access token.");

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not create chatter access token because the config get attempt failed.");
                return null;
            }

            var createdChatter = await Create(config.ChatterClientId, config.ChatterClientSecret, AppCache.StoredChatterRefreshToken, config.ChatterScope, config.TokenExpirationBuffer);
            if (createdChatter is null) {
                Logger.Warning("Could not create chatter access token because the create attempt failed.");
                return null;
            }

            return createdChatter;
        }

        public static async Task<AccessToken?> CreateCollector() {
            Logger.Info("Creating collector access token.");

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not create collector access token because the config get attempt failed.");
                return null;
            }

            var createdCollector = await Create(config.CollectorClientId, config.CollectorClientSecret, AppCache.StoredCollectorRefreshToken, config.CollectorScope, config.TokenExpirationBuffer);
            if (createdCollector is null) {
                Logger.Warning("Could not create collector access token because the create attempt failed.");
                return null;
            }

            return createdCollector;

        }

        public async Task<string?> GetString() {
            Logger.Info("Getting access token string.");

            if (IsAboutToExpire && !await Refresh()) {
                Logger.Warning("Could not get access token string because the access token is about to expire and the refresh attempt failed.");
                return null;
            }

            return accessToken;
        }

        public async Task<bool> Refresh() {
            Logger.Info("Refreshing access token.");

            var potentialData = await RequestRefresh(ClientId, ClientSecret, RefreshToken);
            if (potentialData is null) {
                Logger.Warning("Could not refresh access token because the request refresh attempt failed.");
                return false;
            }

            var data = (AccessTokenData)potentialData;
            ExpirationDate = DateTime.Now.AddSeconds(data.ExpiresIn);
            accessToken = data.AccessToken;
            RefreshToken = data.RefreshToken;
            return true;
        }

        public bool SetExpirationBuffer(int newExpirationBuffer) {
            Logger.Info("Setting access token expiration buffer.");

            if (newExpirationBuffer < 0) {
                Logger.Warning($"Could not set access token expiration buffer because the `{nameof(newExpirationBuffer)}` parameter is less than 0. Context value: {newExpirationBuffer}.");
                return false;
            }

            ExpirationBuffer = newExpirationBuffer;
            return true;
        }

        private string accessToken;

        private AccessToken(string clientId, string clientSecret, AccessTokenData data, int expirationBuffer) {
            if (expirationBuffer < 0) {
                Logger.Error($"Could not construct new access token because the `{nameof(expirationBuffer)}` parameter is less than 0. Context value: {expirationBuffer}.");
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
            if (storedRefreshToken is not null) {
                var potentialRefreshData = await RequestRefresh(clientId, clientSecret, storedRefreshToken);
                if (potentialRefreshData is not null) {
                    AccessToken refreshedAccessToken;
                    try {
                        refreshedAccessToken = new(clientId, clientSecret, (AccessTokenData)potentialRefreshData, expirationBuffer);
                    } catch (Exception e) {
                        Logger.Warning($"Could not create access token because access token construct attempt failed: {e}.");
                        return null;
                    }

                    return refreshedAccessToken;
                }
            }

            var config = await AppCache.Config.Get();
            if (config is null) {
                Logger.Warning("Could not create access token because the config get attempt failed.");
                return null;
            }

            var code = await AuthorizationCode.Create(clientId, scope);
            if (code is null) {
                Logger.Warning("Could not create access token because the authorization code create attempt failed.");
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
                Logger.Warning("Could not create access token because the Twitch get access token attempt failed.");
                return null;
            }

            AccessToken createdAccessToken;
            try {
                createdAccessToken = new(clientId, clientSecret, (AccessTokenData)potentialData, expirationBuffer);
            } catch (Exception e) {
                Logger.Warning($"Could not create access token because AccessToken construct attempt failed: {e}.");
                return null;
            }

            return createdAccessToken;
        }

        private static async Task<AccessTokenData?> RequestRefresh(
            string clientId,
            string clientSecret,
            string refreshToken
        ) => await Util.GetMessageAs<AccessTokenData>(TwitchAPI.RefreshAccessToken(
            new(),
            clientId,
            clientSecret,
            refreshToken
        ));
    }
}
