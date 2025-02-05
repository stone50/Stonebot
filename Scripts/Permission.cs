﻿namespace Stonebot.Scripts {
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
            Logger.Info("Getting highest user permission level.");
            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is not null && broadcaster.Id == userId) {
                return PermissionLevel.Broadcaster;
            }

            var isMod = await GetIsMod(userId);
            if (isMod is not null && (bool)isMod) {
                return PermissionLevel.Mod;
            }

            var subTier = await GetSubTier(userId);
            if (subTier is not null) {
                switch (subTier) {
                    case 1:
                        return PermissionLevel.Tier1Sub;
                    case 2:
                        return PermissionLevel.Tier2Sub;
                    case 3:
                        return PermissionLevel.Tier3Sub;
                }

                Logger.Warning($"Cannot get highest because subscription tier '{subTier}' is not supported.");
                return null;
            }

            var isVIP = await GetIsVIP(userId);
            return isVIP is not null && (bool)isVIP ? PermissionLevel.VIP : PermissionLevel.Viewer;
        }
    }
}
