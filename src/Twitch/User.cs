namespace Stonebot.Twitch {
    using Helpers;
    using Models.Responses;

    internal static class User {
        public static GetUsersResponseDataPoint GetChatter() {
            var users = HttpHelper.SendAuthorizedGetRequest("https://api.twitch.tv/helix/users", JsonContext.Default.GetUsersResponse);
            return users.Data[0];
        }

        public static GetUsersResponseDataPoint GetBroadcaster() {
            var url = HttpHelper.GetUrl("https://api.twitch.tv/helix/users", new() {
                { "login", Config.BroadcasterUsername.ToLower() },
            });
            var users = HttpHelper.SendUnauthorizedGetRequest(url, JsonContext.Default.GetUsersResponse);
            return users.Data[0];
        }
    }
}
