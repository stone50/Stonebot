namespace Stonebot.Twitch {
    using Helpers;
    using Models.Bodies;

    internal static class EventSub {
        public static void DeleteEventSub(string id) {
            var url = HttpHelper.GetUrl("https://api.twitch.tv/helix/eventsub/subscriptions", new() { { "id", id } });
            var cancellationToken = TaskHelper.GetDefaultCancellationToken();
            _ = TaskHelper.Sync(Cache.GetAuthorizedHttpClient().DeleteAsync(url, cancellationToken)).EnsureSuccessStatusCode();
        }

        public static void DeleteEventSubs() {
            var eventSubs = HttpHelper.SendAuthorizedGetRequest("https://api.twitch.tv/helix/eventsub/subscriptions", JsonContext.Default.GetEventSubsResponse);
            foreach (var eventSub in eventSubs.Data) {
                DeleteEventSub(eventSub.Id);
            }
        }

        public static void SubscribeToChannelChatMessage(CancellationToken cancellationToken) {
            var body = new PostAddChannelChatMessageEventSubBody() {
                Type = "channel.chat.message",
                Version = "1",
                Condition = new() {
                    BroadcasterId = Cache.GetBroadcasterId(),
                    UserId = Cache.GetChatterId(),
                },
                Transport = new() {
                    Method = "websocket",
                    SessionId = WebSocketClient.Id!,
                }
            };
            HttpHelper.SendAuthorizedPostRequest("https://api.twitch.tv/helix/eventsub/subscriptions", body, JsonContext.Default.PostAddChannelChatMessageEventSubBody, cancellationToken);
        }
    }
}
