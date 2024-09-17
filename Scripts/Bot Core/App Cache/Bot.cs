namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System.Threading.Tasks;

    internal class Bot : User {
        public static async Task<Bot?> Create() {
            var config = await AppCache.Config.Get();
            if (config is null) {
                return null;
            }

            var potentialData = await GetData(null);
            return potentialData is null ? null : new((UserData)potentialData);
        }

        private Bot(UserData data) : base(data) { }
    }
}
