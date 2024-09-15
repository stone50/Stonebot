namespace StoneBot.Scripts.Bot_Core.Access_Token {
    using App_Cache;
    using Godot;
    using Models;
    using System;
    using System.Threading.Tasks;

    internal class UserAccessToken : AccessToken {
        private string RefreshToken;

        public UserAccessToken(UserAccessTokenData data) : base(data.AccessToken, data.ExpiresIn) => RefreshToken = data.RefreshToken;

        public override async Task<bool> Refresh() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot refresh user access token because configValues is null.");
                return false;
            }

            var configValues = (ConfigValues)potentialConfigValues;
            var potentialEserAccessTokenData = await Util.ProcessHttpResponseMessage<UserAccessTokenData>(await TwitchAPI.RefreshAccessToken(
                new(),
                configValues.BotClientId,
                configValues.BotClientSecret,
                RefreshToken
            ));
            if (potentialEserAccessTokenData is null) {
                GD.PushWarning("Cannot refresh user access token because ProcessHttpResponseMessage failed.");
                return false;
            }

            var userAccessTokenData = (UserAccessTokenData)potentialEserAccessTokenData;
            LastRefreshDate = DateTime.Now;
            AccessTokenString = userAccessTokenData.AccessToken;
            ExpiresIn = userAccessTokenData.ExpiresIn;
            RefreshToken = userAccessTokenData.RefreshToken;
            return true;
        }
    }
}
