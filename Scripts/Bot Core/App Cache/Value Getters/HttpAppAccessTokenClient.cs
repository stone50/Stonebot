namespace StoneBot.Scripts.Bot_Core.App_Cache.Value_Getters {
    using Godot;
    using Http.Access_Token_Client;
    using System.Threading.Tasks;

    internal static partial class ValueGetters {
        public static async Task<HttpAppAccessTokenClient?> GetHttpAppAccessTokenClient() {
            var appAccessToken = await AppCache.AppAccessToken.Get();
            if (appAccessToken is null) {
                GD.PushWarning("Cannot get http app access token client because appAccessToken is null.");
                return null;
            }

            return new(appAccessToken);
        }
    }
}
