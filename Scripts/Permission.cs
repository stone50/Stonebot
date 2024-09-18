namespace StoneBot.Scripts {
    using Bot_Core.App_Cache;
    using System.Threading.Tasks;

    internal static class Permission {
        public enum PermissionLevel {
            Viewer,
            VIP,
            Sub,
            Mod,
            Broadcaster
        }

        public static async Task<PermissionLevel?> GetHighest(string userId) {
            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is not null && broadcaster.Id == userId) {
                return PermissionLevel.Broadcaster;
            }

            // TODO
            return PermissionLevel.Viewer;
        }
    }
}
