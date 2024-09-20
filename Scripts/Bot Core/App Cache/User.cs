namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal class User {
        public string Id { get; private set; }

        public static async Task<User?> Create(string? login, HttpClientWrapper clientWrapper) {
            var client = await clientWrapper.GetClient();
            if (client is null) {
                return null;
            }

            var potentialUsersData = await Util.GetMessageAs<UsersData>(TwitchAPI.GetUsers(client, null, login is null ? null : new[] { login }));
            if (potentialUsersData is null) {
                return null;
            }

            var usersData = (UsersData)potentialUsersData;
            if (usersData.Data.Length == 0) {
                GD.PushWarning($"Cannot create broadcaster becauseusersData.Data.Length is 0.");
                return null;
            }

            return new(usersData.Data[0]);
        }

        public static async Task<User?> CreateBot() {
            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            return clientWrapper is null ? null : await Create(null, clientWrapper);
        }

        public static async Task<User?> CreateBroadcaster() {
            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var clientWrapper = await AppCache.ListenerClientWrapper.Get();
            return clientWrapper is null ? null : await Create(config.BroadcasterLogin, clientWrapper);
        }

        private User(UserData data) => Id = data.Id;
    }
}
