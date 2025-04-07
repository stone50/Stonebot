namespace Stonebot.Scripts {
    using Bot_Core.App_Cache;
    using System.Threading.Tasks;
    using static Core_Interface.User;

    internal enum PermissionLevel {
        Viewer,
        VIP,
        Tier1Sub,
        Tier2Sub,
        Tier3Sub,
        Mod,
        Broadcaster
    }

    internal static class Permission {
        public static async Task<PermissionLevel?> GetHighest(string userId) {
            Logger.Info($"Getting highest user permission level. User id: {userId}.");

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not get highest user permission level because broadcaster get attempt failed.");
                return null;
            }

            if (broadcaster.Id == userId) {
                return PermissionLevel.Broadcaster;
            }

            var isMod = await GetIsMod(userId);
            if (isMod is null) {
                Logger.Warning("Could not get highest user permission level because get is mod attempt failed.");
                return null;
            }

            if ((bool)isMod) {
                return PermissionLevel.Mod;
            }

            var subTier = await GetSubTier(userId);
            if (subTier is null) {
                Logger.Warning("Could not get highest user permission level because get sub tier attempt failed.");
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

                Logger.Warning($"Could not get highest user permission level because sub tier is not supported. Sub tier: {subTier}.");
                return null;
            }

            var isVIP = await GetIsVIP(userId);
            if (isVIP is null) {
                Logger.Warning("Could not get highest user permission level because get is VIP attempt failed.");
                return null;
            }

            return (bool)isVIP ? PermissionLevel.VIP : PermissionLevel.Viewer;
        }
    }
}
