namespace Stonebot.Python {
    public class ChatterPermission(UserPermission.Level permissionLevel) {
        public readonly int level = (int)permissionLevel;
        public readonly string role = permissionLevel switch {
            UserPermission.Level.Viewer => "viewer",
            UserPermission.Level.VIP => "vip",
            UserPermission.Level.Tier1Sub => "tier_1_sub",
            UserPermission.Level.Tier2Sub => "tier_2_sub",
            UserPermission.Level.Tier3Sub => "tier_3_sub",
            UserPermission.Level.Moderator => "moderator",
            UserPermission.Level.Broadcaster => "broadcaster",
            _ => "unknown"
        };
    }
}
