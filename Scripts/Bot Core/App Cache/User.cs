namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Twitch;

    internal class User {
        public string Id { get; private set; }
        public string UserName { get; private set; }

        public static async Task<User?> CreateBot() {
            var logPrefix = $"{nameof(User)} | {nameof(CreateBot)}";
            Logger.Info(logPrefix);

            var clientWrapper = await AppCache.ChatterClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.ChatterClientWrapper.Get)} result is null.");
                return null;
            }

            var createdUser = await Create(clientWrapper);
            if (createdUser is null) {
                Logger.Warning($"{logPrefix} | {nameof(Create)} result is null.\n{nameof(clientWrapper)}: {clientWrapper.MaskedSerialized}");
                return null;
            }

            return createdUser;
        }

        public static async Task<User?> CreateBroadcaster() {
            var logPrefix = $"{nameof(User)} | {nameof(CreateBroadcaster)}";
            Logger.Info(logPrefix);

            var clientWrapper = await AppCache.CollectorClientWrapper.Get();
            if (clientWrapper is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.CollectorClientWrapper.Get)} result is null.");
                return null;
            }

            var createdUser = await Create(clientWrapper);
            if (createdUser is null) {
                Logger.Warning($"{logPrefix} | {nameof(Create)} result is null.\n{nameof(clientWrapper)}: {clientWrapper.MaskedSerialized}");
                return null;
            }

            return createdUser;
        }

        private User(UserData data) {
            Logger.Info($"{nameof(User)} | Constructor\n{nameof(data)}: {JsonSerializer.Serialize(data)}");

            Id = data.Id;
            UserName = data.DisplayName;
        }

        private static async Task<User?> Create(HttpClientWrapper clientWrapper) {
            var logPrefix = $"{nameof(User)} | {nameof(Create)}";
            Logger.Info($"{logPrefix}\n{nameof(clientWrapper)}: {clientWrapper.MaskedSerialized}");

            var client = await clientWrapper.GetClient();
            if (client is null) {
                Logger.Warning($"{logPrefix} | {nameof(clientWrapper.GetClient)} result is null.");
                return null;
            }

            var potentialUsersData = await Util.GetMessageAs<UsersData>(TwitchAPI.GetUsers(client));
            if (potentialUsersData is null) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.GetUsers)} was unsuccessful.");
                return null;
            }

            var usersData = (UsersData)potentialUsersData;
            if (usersData.Data.Length == 0) {
                Logger.Warning($"{logPrefix} | {nameof(TwitchAPI.GetUsers)} was unsuccessful.\n{nameof(usersData)}: {JsonSerializer.Serialize(usersData)}");
                return null;
            }

            return new(usersData.Data[0]);
        }
    }
}
