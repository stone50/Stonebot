namespace StoneBot.Scripts.App_Cache.Value_Getters {
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal static partial class ValueGetters {
        public static async Task<string?> GetBroadcasterId() {
            var potentialConfigValues = await AppCache.ConfigValues.Get();
            if (potentialConfigValues is null) {
                GD.PushWarning("Cannot get broadcaster id because configValues is null.");
                return null;
            }

            var configValues = (ConfigValues)potentialConfigValues;
            var httpUserAccessTokenClient = await AppCache.HttpUserAccessTokenClient.Get();
            if (httpUserAccessTokenClient is null) {
                GD.PushWarning("Cannot get broadcaster id because httpUserAccessTokenClient is null.");
                return null;
            }

            var client = await httpUserAccessTokenClient.GetClient();
            if (client is null) {
                GD.PushWarning("Cannot get broadcaster id because httpUserAccessTokenClient.GetClient failed.");
                return null;
            }

            var potentialUsersData = await Util.ProcessHttpResponseMessage<UsersData>(await TwitchAPI.GetUsers(client, null, new[] { configValues.BroadcasterLogin }));
            if (potentialUsersData is null) {
                GD.PushWarning("Cannot get broadcaster id because ProcessHttpResponseMessage failed.");
                return null;
            }

            var usersData = (UsersData)potentialUsersData;
            if (usersData.Data.Length == 0) {
                GD.PushWarning($"Cannot get broadcaster id because GetUsers found no users.");
                return null;
            }

            return usersData.Data[0].Id;
        }
    }
}
