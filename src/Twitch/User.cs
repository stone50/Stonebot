namespace Stonebot.Twitch {
    using Models.Responses;
    using System.Threading;

    internal static class User {
        public static GetUsersResponseDataPoint GetUser(AccessToken accessToken, CancellationToken cancellationToken) {
            var client = accessToken.GetHttpClient(cancellationToken);
            var users = Utils.SendGetRequest(client, "https://api.twitch.tv/helix/users", JsonContext.Default.GetUsersResponse, cancellationToken);
            return users.Data[0];
        }

        public static bool GetIsModerator(string userId, CancellationToken cancellationToken) {
            var client = Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClient(cancellationToken);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/moderation/moderators", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });
            var mods = Utils.SendGetRequest(client, url, JsonContext.Default.GetModeratorsResponse, cancellationToken);
            return mods.Data.Length != 0;
        }

        public static bool GetIsVIP(string userId, CancellationToken cancellationToken) {
            var client = Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClient(cancellationToken);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/channels/vips", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });

            var vips = Utils.SendGetRequest(client, url, JsonContext.Default.GetVIPsResponse, cancellationToken);
            return vips.Data.Length != 0;
        }

        public static int GetSubscriptionTier(string userId, CancellationToken cancellationToken) {
            var client = Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClient(cancellationToken);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/subscriptions/user", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });

            var subscriptions = Utils.SendGetRequest(client, url, JsonContext.Default.GetSubscriptionResponse, cancellationToken);
            return subscriptions.Data.Length == 0 ? 0 : subscriptions.Data[0].Tier switch {
                "1000" => 1,
                "2000" => 2,
                "3000" => 3,
                _ => -1,
            };
        }
    }
}
