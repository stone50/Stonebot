namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal class Broadcaster {
        public string Id { get; private set; }

        public static async Task<Broadcaster?> Create() {
            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var clientWrapper = await AppCache.HttpClientWrapper.Get();
            if (clientWrapper is null) {
                return null;
            }

            var client = await clientWrapper.GetClient();
            if (client is null) {
                return null;
            }

            var potentialUsersData = await Util.GetMessageAs<UsersData>(TwitchAPI.GetUsers(client, null, new[] { config.BroadcasterLogin }));
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

        private Broadcaster(UserData data) => Id = data.Id;
    }
}
