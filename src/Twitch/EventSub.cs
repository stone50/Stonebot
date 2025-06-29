namespace Stonebot.Twitch {
    using Models;
    using System.Threading;

    internal static class EventSub {
        public static void DeleteEventSub(string id, CancellationToken cancellationToken) {
            var client = Cache.ChatterAuthorizationData!.AccessToken.GetHttpClient(cancellationToken);
            var url = Utils.GetUrl("https://api.twitch.tv/helix/eventsub/subscriptions", new() { { "id", id } });
            var deleteTask = client.DeleteAsync(url, cancellationToken);
            var response = Utils.Sync(deleteTask);
            _ = response.EnsureSuccessStatusCode();
        }

        public static void DeleteEventSubs(CancellationToken cancellationToken) {
            var client = Cache.ChatterAuthorizationData!.AccessToken.GetHttpClient(cancellationToken);
            var eventSubs = Utils.SendGetRequest(client, "https://api.twitch.tv/helix/eventsub/subscriptions", JsonContext.Default.GetEventSubsResponse, cancellationToken);
            foreach (var eventSub in eventSubs.Data) {
                DeleteEventSub(eventSub.Id, cancellationToken);
            }
        }

        public static void SubscribeToChannelChatMessage(CancellationToken cancellationToken) {
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
            var client = Cache.ChatterAuthorizationData!.AccessToken.GetHttpClient(cancellationToken);
            Utils.SendPostRequest(client, "https://api.twitch.tv/helix/eventsub/subscriptions", content, JsonContext.Default.AddChannelChatMessageEventSub, cancellationToken);
        }
    }
}
