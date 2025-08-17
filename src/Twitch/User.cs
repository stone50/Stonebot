namespace Stonebot.Twitch {
    using Models.Responses;

    internal static class User {
        public static GetUsersResponseDataPoint GetChatter() {
            var users = Utils.SendAuthorizedGetRequest("https://api.twitch.tv/helix/users", JsonContext.Default.GetUsersResponse);
            return users.Data[0];
        }

        public static GetUsersResponseDataPoint GetBroadcaster() {
            var url = Utils.GetUrl("https://api.twitch.tv/helix/users", new() {
                { "login", Config.BroadcasterUsername.ToLower() },
            });
            var users = Utils.SendUnauthorizedGetRequest(url, JsonContext.Default.GetUsersResponse);
            return users.Data[0];
        }
    }
}
