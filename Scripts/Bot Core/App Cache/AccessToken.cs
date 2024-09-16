namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models;
    using System;
    using System.Threading.Tasks;

    internal class AccessToken {
        public DateTime ExpirationDate { get; protected set; }
        public int ExpirationBuffer { get => expirationBuffer; set => SetExpirationBuffer(value); }

        public bool IsAboutToExpire => DateTime.Now.AddMilliseconds(ExpirationBuffer) >= ExpirationDate;

        public static async Task<AccessToken?> Create() {
            var configValues = await AppCache.ConfigValues.Get();
            if (configValues is null) {
                return null;
            }

            var code = await AuthorizationCode.Create();
            if (code is null) {
                return null;
            }

            var potentialData = await Util.GetMessageAs<AccessTokenData>(TwitchAPI.GetAccessToken(
                new(),
                configValues.BotClientId,
                configValues.BotClientSecret,
                code,
                $"http://localhost:{configValues.AuthorizationPort}"
            ));
            return potentialData is null ? null : new((AccessTokenData)potentialData);
        }

        public async Task<string?> GetString() => IsAboutToExpire && !await Refresh() ? null : accessToken;

        public async Task<bool> Refresh() {
            var configValues = await AppCache.ConfigValues.Get();
            if (configValues is null) {
                return false;
            }

            var potentialData = await Util.GetMessageAs<AccessTokenData>(TwitchAPI.RefreshAccessToken(
                new(),
                configValues.BotClientId,
                configValues.BotClientSecret,
                refreshToken
            ));
            if (potentialData is null) {
                return false;
            }

            var data = (AccessTokenData)potentialData;
            ExpirationDate = DateTime.Now.AddMilliseconds(data.ExpiresIn);
            accessToken = data.AccessToken;
            refreshToken = data.RefreshToken;
            return true;
        }

        private string accessToken;
        private string refreshToken;
        private int expirationBuffer = 1000;

        private AccessToken(AccessTokenData data) {
            accessToken = data.AccessToken;
            refreshToken = data.RefreshToken;
            ExpirationDate = DateTime.Now.AddMilliseconds(data.ExpiresIn);
        }

        private void SetExpirationBuffer(int newExpirationBuffer) {
            if (newExpirationBuffer < 0) {
                GD.PushWarning("Cannot set expiration buffer because newExpirationBuffer is less than 0.");
                throw new ArgumentOutOfRangeException(nameof(newExpirationBuffer));
            }

            expirationBuffer = newExpirationBuffer;
        }
    }
}
