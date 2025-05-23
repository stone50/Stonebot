namespace Stonebot.Twitch {
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class EventSub {
        public static async Task DeleteEventSubAsync(string id, CancellationToken cancellationToken) {
            var client = await Cache.ChatterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/eventsub/subscriptions", new() { { "id", id } });
            var response = await client.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
            _ = response.EnsureSuccessStatusCode();
        }

        public static async Task DeleteEventSubsAsync(CancellationToken cancellationToken) {
            var client = await Cache.ChatterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            var eventSubs = await Utils.SendGetRequestAsync(client, "https://api.twitch.tv/helix/eventsub/subscriptions", JsonContext.Default.GetEventSubsResponse, cancellationToken).ConfigureAwait(false);
            foreach (var eventSub in eventSubs.Data) {
                await DeleteEventSubAsync(eventSub.Id, cancellationToken);
            }
        }

        public static async Task SubscribeToChannelChatMessageAsync(CancellationToken cancellationToken) {
            var content = new AddChannelChatMessageEventSub() {
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
            var client = await Cache.ChatterAuthorizationData!.AccessToken.GetHttpClientAsync(cancellationToken).ConfigureAwait(false);
            await Utils.SendPostRequestAsync(client, "https://api.twitch.tv/helix/eventsub/subscriptions", content, JsonContext.Default.AddChannelChatMessageEventSub, cancellationToken).ConfigureAwait(false);
        }
    }
}
