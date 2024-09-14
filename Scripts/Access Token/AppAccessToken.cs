namespace StoneBot.Scripts.Access_Token {
    using App_Cache.Value_Getters;
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal class AppAccessToken : AccessToken {
        public AppAccessToken(AppAccessTokenData data) : base(data.AccessToken, data.ExpiresIn) { }

        public override async Task<bool> Refresh() {
            var appAccessToken = await ValueGetters.GetAppAccessToken();
            if (appAccessToken is null) {
                GD.PushWarning("Cannot refresh app access token because GetAppAccessToken failed.");
                return false;
            }

            CreationDate = appAccessToken.CreationDate;
            AccessTokenString = appAccessToken.AccessTokenString;
            ExpiresIn = appAccessToken.ExpiresIn;
            return true;
        }
    }
}
