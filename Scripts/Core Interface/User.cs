namespace Stonebot.Scripts.Core_Interface {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models;
    using Bot_Core.Twitch;
    using System.Threading.Tasks;

    internal static class User {
        public static async Task<bool?> GetIsMod(string userId) {
            Logger.Info("Getting is user mod.");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not get is user mod because broadcaster get attempt failed.");
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not get is user mod because collector client wrapper get attempt failed.");
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning("Could not get is user mod because client wrapper get client attempt failed.");
                return null;
            }

            var simpleUsersData = await Util.GetMessageAs<PaginatedSimpleUsersData>(TwitchAPI.GetModerators(
                client,
                broadcaster.Id,
                [userId]
            ));
            if (simpleUsersData is null) {
                Logger.Warning("Could not get is user mod because Twitch API get moderators attempt failed.");
                return null;
            }

            return ((PaginatedSimpleUsersData)simpleUsersData).Data.Length == 1;
        }

        public static async Task<bool?> GetIsVIP(string userId) {
            Logger.Info("Getting is user VIP.");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not get is user VIP because broadcaster get attempt failed.");
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not get is user VIP because collector client wrapper get attempt failed.");
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning("Could not get is user VIP because client wrapper get client attempt failed.");
                return null;
            }

            var simpleUsersData = await Util.GetMessageAs<PaginatedSimpleUsersData>(TwitchAPI.GetVIPs(
                client,
                broadcaster.Id,
                [userId]
            ));
            if (simpleUsersData is null) {
                Logger.Warning("Could not get is user VIP because Twitch API get VIPs attempt failed.");
                return null;
            }

            return ((PaginatedSimpleUsersData)simpleUsersData).Data.Length == 1;
        }

        public static async Task<int?> GetSubTier(string userId) {
            Logger.Info("Getting user subscription tier.");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not get user subscription tier because broadcaster get attempt failed.");
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not get user subscription tier because collector client wrapper get attempt failed.");
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning("Could not get user subscription tier because client wrapper get client attempt failed.");
                return null;
            }

            var potentialSubscriptionsData = await Util.GetMessageAs<PaginatedSubscriptionsData>(TwitchAPI.GetBroadcasterSubscriptions(
                client,
                broadcaster.Id,
                [userId]
            ));
            if (potentialSubscriptionsData is null) {
                Logger.Warning("Could not get user subscription tier because Twitch API get broadcaster subscriptions attempt failed.");
                return null;
            }

            var subscriptionsData = (PaginatedSubscriptionsData)potentialSubscriptionsData;
            return subscriptionsData.Data.Length == 0
                ? 0
                : subscriptionsData.Data[0].Tier switch {
                    "1000" => 1,
                    "2000" => 2,
                    "3000" => 3,
                    _ => null,
                };
        }
    }
}
