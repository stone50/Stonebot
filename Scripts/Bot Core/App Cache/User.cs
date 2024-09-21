namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models;
    using System.Threading.Tasks;
    using Twitch;

    internal class User {
        public string Id { get; private set; }

        public static async Task<User?> Create(HttpClientWrapper clientWrapper) {
            var client = await clientWrapper.GetClient();
            if (client is null) {
                return null;
            }

            var potentialUsersData = await Util.GetMessageAs<UsersData>(TwitchAPI.GetUsers(client));
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
            return clientWrapper is null ? null : await Create(clientWrapper);
        }

        public static async Task<User?> CreateBroadcaster() {
            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            return clientWrapper is null ? null : await Create(clientWrapper);
        }

        private User(UserData data) => Id = data.Id;
    }
}
