namespace StoneBot.Scripts.Bot_Core.App_Cache.Value_Getters {
    using Godot;
    using Http.Access_Token_Client;
    using System.Threading.Tasks;

    internal static partial class ValueGetters {
        public static async Task<HttpUserAccessTokenClient?> GetHttpUserAccessTokenClient() {
            var userAccessToken = await AppCache.UserAccessToken.Get();
            if (userAccessToken is null) {
                GD.PushWarning("Cannot get http user access token client because userAccessToken is null.");
                return null;
            }

            return new(userAccessToken);
        }
    }
}
