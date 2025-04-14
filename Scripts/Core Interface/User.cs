namespace Stonebot.Scripts.Core_Interface {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models;
    using Bot_Core.Twitch;
    using System.Threading.Tasks;

    internal static class User {
        internal enum PermissionLevel {
            Viewer,
            VIP,
            Tier1Sub,
            Tier2Sub,
            Tier3Sub,
            Mod,
            Broadcaster
        }

        public static async Task<PermissionLevel?> GetHighestPermissionLevel(string userId) {
            var logPrefix = $"{nameof(User)} | {nameof(GetHighestPermissionLevel)}";
            Logger.Info($"{logPrefix}\n{nameof(userId)}: {userId}");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
                return null;
            }

            if (broadcaster.Id == userId) {
                return PermissionLevel.Broadcaster;
            }

            var isMod = await GetIsMod(userId);
            if (isMod is null) {
                Logger.Warning($"{logPrefix} | {nameof(GetIsMod)} result is null.");
                return null;
            }

            if ((bool)isMod) {
                return PermissionLevel.Mod;
            }

            var subTier = await GetSubTier(userId);
            if (subTier is null) {
                Logger.Warning($"{logPrefix} | {nameof(GetSubTier)} result is null.");
                return null;
            }

            if (subTier != 0) {
                switch (subTier) {
                    case 1:
                        return PermissionLevel.Tier1Sub;
                    case 2:
                        return PermissionLevel.Tier2Sub;
                    case 3:
                        return PermissionLevel.Tier3Sub;
                }

                Logger.Warning($"{logPrefix} | {nameof(subTier)} not supported.\n{nameof(subTier)}: {subTier}");
                return null;
            }

            var isVIP = await GetIsVIP(userId);
            if (isVIP is null) {
                Logger.Warning($"{logPrefix} | {nameof(GetIsVIP)} result is null.");
                return null;
            }

            return (bool)isVIP ? PermissionLevel.VIP : PermissionLevel.Viewer;
        }

        public static async Task<bool?> GetIsMod(string userId) {
            var logPrefix = $"{nameof(User)} | {nameof(GetIsMod)}";
            Logger.Info($"{logPrefix}\n{nameof(userId)}: {userId}");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.CollectorClientWrapper.Get)} result is null.");
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                return null;
            }

            var simpleUsersData = await Util.GetMessageAs<PaginatedSimpleUsersData>(TwitchAPI.GetModerators(
                client,
                broadcaster.Id,
                [userId]
            ));
            if (simpleUsersData is null) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.GetModerators)} was unsuccessful.");
                return null;
            }

            return ((PaginatedSimpleUsersData)simpleUsersData).Data.Length == 1;
        }

        public static async Task<bool?> GetIsVIP(string userId) {
            var logPrefix = $"{nameof(User)} | {nameof(GetIsVIP)}";
            Logger.Info($"{logPrefix}\n{nameof(userId)}: {userId}");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.CollectorClientWrapper.Get)} result is null.");
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                return null;
            }

            var simpleUsersData = await Util.GetMessageAs<PaginatedSimpleUsersData>(TwitchAPI.GetVIPs(
                client,
                broadcaster.Id,
                [userId]
            ));
            if (simpleUsersData is null) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.GetVIPs)} was unsuccessful.");
                return null;
            }

            return ((PaginatedSimpleUsersData)simpleUsersData).Data.Length == 1;
        }

        public static async Task<int?> GetSubTier(string userId) {
            var logPrefix = $"{nameof(User)} | {nameof(GetSubTier)}";
            Logger.Info($"{logPrefix}\n{nameof(userId)}: {userId}");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
                return null;
            }

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.CollectorClientWrapper.Get)} result is null.");
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                return null;
            }

            var potentialSubscriptionsData = await Util.GetMessageAs<PaginatedSubscriptionsData>(TwitchAPI.GetBroadcasterSubscriptions(
                client,
                broadcaster.Id,
                [userId]
            ));
            if (potentialSubscriptionsData is null) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.GetBroadcasterSubscriptions)} was unsuccessful.");
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
