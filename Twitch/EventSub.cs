namespace Stonebot.Twitch {
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class EventSub {
        public static Task<EventSubsData> GetEventSubsAsync(string? after, CancellationToken cancellationToken) => GetEventSubsAsync(null, after, cancellationToken);

        public static Task<EventSubsData> GetEventSubsByStatusAsync(string status, string? after, CancellationToken cancellationToken) => GetEventSubsAsync(new() { { "status", status } }, after, cancellationToken);

        public static Task<EventSubsData> GetEventSubsByTypeAsync(string type, string? after, CancellationToken cancellationToken) => GetEventSubsAsync(new() { { "type", type } }, after, cancellationToken);

        public static Task<EventSubsData> GetEventSubsByUserIdAsync(string userId, string? after, CancellationToken cancellationToken) => GetEventSubsAsync(new() { { "user_id", userId } }, after, cancellationToken);

        public static async Task DeleteEventSubAsync(string id, CancellationToken cancellationToken) {
            var client = await Cache.ChatterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/eventsub/subscriptions", new() { { "id", id } });
            var response = await client.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
            _ = response.EnsureSuccessStatusCode();
        }

        public static async Task DeleteEventSubsAsync(CancellationToken cancellationToken) {
            // TODO: delete all at once
            var eventSubsData = await GetEventSubsAsync(null, cancellationToken).ConfigureAwait(false);
            foreach (var eventSubData in eventSubsData.Data) {
                await DeleteEventSubAsync(eventSubData.Id, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task<EventSubsData> SubscribeToChannelChatMessageAsync(CancellationToken cancellationToken) {
            var content = new AddChannelChatMessageEventSubContent() {
                Type = "channel.chat.message",
                Version = "1",
                Condition = new() {
                    BroadcasterId = Cache.BroadcasterAuthorizationData!.UserId,
                    UserId = Cache.ChatterAuthorizationData!.UserId,
                },
                Transport = new() {
                    Method = "websocket",
                    SessionId = WebSocketClient.Id!,
                }
            };
            var client = await Cache.ChatterAuthorizationData.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            return await Utils.SendPostRequestAsync(client, "https://api.twitch.tv/helix/eventsub/subscriptions", content, JsonContext.Default.AddChannelChatMessageEventSubContent, JsonContext.Default.EventSubsData, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<EventSubsData> GetEventSubsAsync(Dictionary<string, string>? queryParams, string? after, CancellationToken cancellationToken) {
            var client = await Cache.ChatterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var allQueryParams = queryParams is null ? [] : new Dictionary<string, string>(queryParams);
            if (after is not null) {
                allQueryParams["after"] = after;
            }

            var url = Utils.GetUrl("https://api.twitch.tv/helix/eventsub/subscriptions", allQueryParams);
            return await Utils.SendGetRequestAsync(client, url, JsonContext.Default.EventSubsData, cancellationToken).ConfigureAwait(false);
        }
    }
}
