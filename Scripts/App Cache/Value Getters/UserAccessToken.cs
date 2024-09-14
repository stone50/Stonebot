namespace StoneBot.Scripts.App_Cache.Value_Getters {
    using Access_Token;
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal static partial class ValueGetters {
        public static async Task<UserAccessToken?> GetUserAccessToken() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot get user access token because configValues is null.");
                return null;
            }

            var configValues = (ConfigValues)potentialConfigValues;
            var authorizationCode = await AppCache.AuthorizationCode.Get();
            if (authorizationCode is null) {
                GD.PushWarning("Cannot get user access token because authorizationCode is null.");
                return null;
            }

            var userAccessTokenData = await Util.ProcessHttpResponseMessage<UserAccessTokenData>(
                await TwitchAPI.GetUserAccessToken(
                    new(),
                    configValues.BotClientId,
                    configValues.BotClientSecret,
                    authorizationCode,
                    $"http://localhost:{configValues.AuthorizationPort}"
                )
            );
            if (userAccessTokenData is null) {
                GD.PushWarning("Cannot get user access token because ProcessHttpResponseMessage failed.");
                return null;
            }

            return new((UserAccessTokenData)userAccessTokenData);
        }
    }
}
