namespace Stonebot.Twitch {
    using Models.Responses;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class User {
        public static async Task<GetUsersResponseDataPoint> GetUserAsync(AccessToken accessToken, CancellationToken cancellationToken) {
            var client = await accessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var users = await Utils.SendGetRequestAsync(client, "https://api.twitch.tv/helix/users", JsonContext.Default.GetUsersResponse, cancellationToken).ConfigureAwait(false);
            return users.Data[0];
        }

        public static async Task<bool> GetIsModeratorAsync(string userId, CancellationToken cancellationToken) {
            var client = await Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/moderation/moderators", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });
            var mods = await Utils.SendGetRequestAsync(client, url, JsonContext.Default.GetModeratorsResponse, cancellationToken).ConfigureAwait(false);
            return mods.Data.Length != 0;
        }

        public static async Task<bool> GetIsVIPAsync(string userId, CancellationToken cancellationToken) {
            var client = await Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/channels/vips", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });

            var vips = await Utils.SendGetRequestAsync(client, url, JsonContext.Default.GetVIPsResponse, cancellationToken).ConfigureAwait(false);
            return vips.Data.Length != 0;
        }

        public static async Task<int> GetSubscriptionTier(string userId, CancellationToken cancellationToken) {
            var client = await Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/subscriptions/user", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });

            var subscriptions = await Utils.SendGetRequestAsync(client, url, JsonContext.Default.GetSubscriptionResponse, cancellationToken).ConfigureAwait(false);
            return subscriptions.Data.Length == 0 ? 0 : subscriptions.Data[0].Tier switch {
                "1000" => 1,
                "2000" => 2,
                "3000" => 3,
                _ => -1,
            };
        }
    }
}
