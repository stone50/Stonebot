namespace StoneBot.Scripts.Bot_Core.App_Cache.Value_Getters {
    using Access_Token;
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal static partial class ValueGetters {
        public static async Task<AppAccessToken?> GetAppAccessToken() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot get app access token because configValues is null.");
                return null;
            }

            var configValues = (ConfigValues)potentialConfigValues;
            var appAccessTokenData = await Util.ProcessHttpResponseMessage<AppAccessTokenData>(
                await TwitchAPI.GetAppAccessToken(
                    new(),
                    configValues.BotClientId,
                    configValues.BotClientSecret
                )
            );
            if (appAccessTokenData is null) {
                GD.PushWarning("Cannot get app access token because ProcessHttpResponseMessage failed.");
                return null;
            }

            return new((AppAccessTokenData)appAccessTokenData);
        }
    }
}
