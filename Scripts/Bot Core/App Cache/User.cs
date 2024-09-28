namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System.Threading.Tasks;
    using Twitch;

    internal class User {
        public string Id { get; private set; }
        public string UserName { get; private set; }

        public static async Task<User?> CreateBot() {
            Logger.Info("Creating bot user.");
            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            return clientWrapper is null ? null : await Create(clientWrapper);
        }

        public static async Task<User?> CreateBroadcaster() {
            Logger.Info("Creating broadcaster user.");
            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            return clientWrapper is null ? null : await Create(clientWrapper);
        }

        private User(UserData data) {
            Id = data.Id;
            UserName = data.DisplayName;
        }

        private static async Task<User?> Create(HttpClientWrapper clientWrapper) {
            Logger.Info("Creating user.");
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
                Logger.Warning($"Cannot create user because usersData.Data.Length is 0.");
                return null;
            }

            return new(usersData.Data[0]);
        }
    }
}
