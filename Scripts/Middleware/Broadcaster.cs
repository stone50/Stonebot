namespace StoneBot.Scripts.Middleware {
    using App_Cache;
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal static class Broadcaster {
        public static async Task<UserData?> GetBroadcasterData() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot get broadcaster data because configValues is null.");
                return null;
            }

            var configValues = (ConfigValues)potentialConfigValues;
            var httpUserAccessTokenClient = await AppCache.HttpUserAccessTokenClient.Get();
            if (httpUserAccessTokenClient is null) {
                GD.PushWarning("Cannot get broadcaster data because httpUserAccessTokenClient is null.");
                return null;
            }

            var client = await httpUserAccessTokenClient.GetClient();
            if (client is null) {
                GD.PushWarning("Cannot get broadcaster data because httpUserAccessTokenClient.GetClient failed.");
                return null;
            }

            var potentialUsersData = await Util.ProcessHttpResponseMessage<UsersData>(await TwitchAPI.GetUsers(client, null, new[] { configValues.BroadcasterLogin }));
            if (potentialUsersData is null) {
                GD.PushWarning("Cannot get broadcaster data because ProcessHttpResponseMessage failed.");
                return null;
            }

            var usersData = (UsersData)potentialUsersData;
            if (usersData.Data.Length == 0) {
                GD.PushWarning($"Cannot get broadcaster data because GetUsers found no users.");
                return null;
            }

            return usersData.Data[0];
        }
    }
}
