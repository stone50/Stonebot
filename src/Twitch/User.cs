namespace Stonebot.Twitch {
    using Models.Responses;
    using System.Threading;

    internal static class User {
        public static GetUsersResponseDataPoint GetUser(AccessToken accessToken, CancellationToken cancellationToken) {
            var client = accessToken.GetHttpClient(cancellationToken);
            var users = Utils.SendGetRequest(client, "https://api.twitch.tv/helix/users", JsonContext.Default.GetUsersResponse, cancellationToken);
            return users.Data[0];
        }
    }
}
