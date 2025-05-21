namespace Stonebot.Twitch {
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class User {
        public static async Task<UserData> GetUserAsync(AccessToken accessToken, CancellationToken cancellationToken) {
            var client = await accessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var usersData = await Utils.SendGetRequestAsync(client, "https://api.twitch.tv/helix/users", JsonContext.Default.UsersData, cancellationToken).ConfigureAwait(false);
            return usersData.Data[0];
        }

        public static async Task<bool> GetIsModeratorAsync(string userId, CancellationToken cancellationToken) {
            var client = await Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/moderation/moderators", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });
            var usersData = await Utils.SendGetRequestAsync(client, url, JsonContext.Default.UsersData, cancellationToken).ConfigureAwait(false);
            return usersData.Data.Length != 0;
        }

        public static async Task<bool> GetIsVIPAsync(string userId, CancellationToken cancellationToken) {
            var client = await Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/channels/vips", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });

            var usersData = await Utils.SendGetRequestAsync(client, url, JsonContext.Default.UsersData, cancellationToken).ConfigureAwait(false);
            return usersData.Data.Length != 0;
        }

        public static async Task<int> GetSubscriptionTeir(string userId, CancellationToken cancellationToken) {
            var client = await Cache.BroadcasterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/subscriptions", new() {
                { "broadcaster_id",  Cache.BroadcasterAuthorizationData.UserId },
                { "user_id",  userId },
            });

            var paginatedSubscriptionsData = await Utils.SendGetRequestAsync(client, url, JsonContext.Default.PaginatedSubscriptionsData, cancellationToken).ConfigureAwait(false);
            var subscriptions = paginatedSubscriptionsData.Data;
            return subscriptions.Length == 0 ? 0 : subscriptions[0].Tier switch {
                "1000" => 1,
                "2000" => 2,
                "3000" => 3,
                _ => 0,
            };
        }
    }
}
