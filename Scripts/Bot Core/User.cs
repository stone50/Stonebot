namespace StoneBot.Scripts.Bot_Core {
    using App_Cache;
    using Godot;
    using Models;
    using System.Threading.Tasks;

    internal abstract class User {
        public string Id { get; private set; }

        protected static async Task<UserData?> GetData(string? login) {
            var clientWrapper = await AppCache.HttpClientWrapper.Get();
            if (clientWrapper is null) {
                return null;
            }

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

            return usersData.Data[0];
        }

        protected User(UserData data) => Id = data.Id;
    }
}
