namespace Stonebot.Twitch {
    using Models.Bodies;

    internal static class EventSub {
        public static void DeleteEventSub(string id) {
            var url = Utils.GetUrl("https://api.twitch.tv/helix/eventsub/subscriptions", new() { { "id", id } });
            var cancellationToken = Utils.GetDefaultCancellationToken();
            _ = Utils.Sync(Cache.AuthorizedHttpClient.DeleteAsync(url, cancellationToken)).EnsureSuccessStatusCode();
        }

        public static void DeleteEventSubs() {
            var eventSubs = Utils.SendAuthorizedGetRequest("https://api.twitch.tv/helix/eventsub/subscriptions", JsonContext.Default.GetEventSubsResponse);
            foreach (var eventSub in eventSubs.Data) {
                DeleteEventSub(eventSub.Id);
            }
        }

        public static void SubscribeToChannelChatMessage() {
            var body = new AddChannelChatMessageEventSubBody() {
                Type = "channel.chat.message",
                Version = "1",
                Condition = new() {
                    BroadcasterId = Cache.BroadcasterId,
                    UserId = Cache.ChatterId,
                },
                Transport = new() {
                    Method = "websocket",
                    SessionId = WebSocketClient.Id!,
                }
            };
            Utils.SendAuthorizedPostRequest("https://api.twitch.tv/helix/eventsub/subscriptions", body, JsonContext.Default.AddChannelChatMessageEventSubBody);
        }
    }
}
