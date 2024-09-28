namespace StoneBot.Scripts.Core_Interface {
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
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                return null;
            }

            var simpleUsersData = await Util.GetMessageAs<PaginatedSimpleUsersData>(TwitchAPI.GetModerators(
                client,
                broadcaster.Id,
                new[] { userId }
            ));
            return simpleUsersData is null ? null : ((PaginatedSimpleUsersData)simpleUsersData).Data.Length == 1;
        }

        public static async Task<bool?> GetIsVIP(string userId) {
            Logger.Info("Getting is user VIP.");
            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                return null;
            }

            var simpleUsersData = await Util.GetMessageAs<PaginatedSimpleUsersData>(TwitchAPI.GetVIPs(
                client,
                broadcaster.Id,
                new[] { userId }
            ));
            return simpleUsersData is null ? null : ((PaginatedSimpleUsersData)simpleUsersData).Data.Length == 1;
        }

        public static async Task<SimpleSubscriptionData?> CheckUserSubscriptions(string userId) {
            Logger.Info("Checking user subscriptions.");
            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                return null;
            }

            var potentialSubscriptionsData = await Util.GetMessageAs<SimpleSubscriptionsData>(TwitchAPI.CheckUserSubscription(
                client,
                broadcaster.Id,
                userId
            ));
            if (potentialSubscriptionsData is null) {
                return null;
            }

            var subscriptionsData = (SimpleSubscriptionsData)potentialSubscriptionsData;
            return subscriptionsData.Data.Length == 0 ? null : subscriptionsData.Data[0];
        }
    }
}
