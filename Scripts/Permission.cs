namespace StoneBot.Scripts {
    using Bot_Core.App_Cache;
    using Bot_Core.Models;
    using Godot;
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
            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is not null && broadcaster.Id == userId) {
                return PermissionLevel.Broadcaster;
            }

            var isMod = await GetIsMod(userId);
            if (isMod is not null && (bool)isMod) {
                return PermissionLevel.Mod;
            }

            var potentialSubData = await CheckUserSubscriptions(userId);
            if (potentialSubData is not null) {
                var tier = ((SimpleSubscriptionData)potentialSubData).Tier;
                switch (tier) {
                    case "1":
                        return PermissionLevel.Tier1Sub;
                    case "2":
                        return PermissionLevel.Tier2Sub;
                    case "3":
                        return PermissionLevel.Tier3Sub;
                }

                GD.PushWarning($"Cannot get highest because subscription tier '{tier}' is not supported.");
                return null;
            }

            var isVIP = await GetIsVIP(userId);
            return isVIP is not null && (bool)isVIP ? PermissionLevel.VIP : PermissionLevel.Viewer;
        }
    }
}
