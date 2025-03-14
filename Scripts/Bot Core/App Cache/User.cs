namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System.Threading.Tasks;
    using Twitch;

    internal class User {
        public string Id { get; private set; }
        public string UserName { get; private set; }

        public static async Task<User?> CreateBot() {
            Logger.Info("Creating bot user.");

            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not create bot user because chatter client wrapper get attempt failed.");
                return null;
            }

            var createdUser = await Create(clientWrapper);
            if (createdUser is null) {
                Logger.Warning("Could not create bot user because create attempt failed.");
                return null;
            }

            return createdUser;
        }

        public static async Task<User?> CreateBroadcaster() {
            Logger.Info("Creating broadcaster user.");

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning("Could not create broadcaster user because collector client wrapper get attempt failed.");
                return null;
            }

            var createdUser = await Create(clientWrapper);
            if (createdUser is null) {
                Logger.Warning("Could not create broadcaster user because create attempt failed.");
                return null;
            }

            return createdUser;
        }

        private User(UserData data) {
            Id = data.Id;
            UserName = data.DisplayName;
        }

        private static async Task<User?> Create(HttpClientWrapper clientWrapper) {
            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning("Could not create user because client wrapper get client attempt failed.");
                return null;
            }

            var potentialUsersData = await Util.GetMessageAs<UsersData>(TwitchAPI.GetUsers(client));
            if (potentialUsersData is null) {
                Logger.Warning("Could not create user because Twitch get users attempt failed.");
                return null;
            }

            var usersData = (UsersData)potentialUsersData;
            if (usersData.Data.Length == 0) {
                Logger.Warning("Could not create user because Twitch get users attempt failed.");
                return null;
            }

            return new(usersData.Data[0]);
        }
    }
}
