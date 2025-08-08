namespace Stonebot.Python {
    public class ChatterPermission(UserPermission.Level permissionLevel) {
        public readonly int level = (int)permissionLevel;
        public readonly string role = permissionLevel switch {
            UserPermission.Level.Viewer => "viewer",
            UserPermission.Level.Subscriber => "subscriber",
            UserPermission.Level.VIP => "vip",
            UserPermission.Level.Moderator => "moderator",
            UserPermission.Level.Broadcaster => "broadcaster",
            _ => "unknown"
        };
    }
}
