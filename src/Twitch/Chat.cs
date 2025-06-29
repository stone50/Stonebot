namespace Stonebot.Twitch {
    using Models;

    internal static class Chat {
        public static SendChatMessageResponse Send(string message, string? replyParentMessageId, CancellationToken cancellationToken) {
            var client = Cache.ChatterAuthorizationData!.AccessToken.GetHttpClient(cancellationToken);
            var body = new SendChatMessage {
                BroadcasterId = Cache.BroadcasterAuthorizationData!.UserId,
                SenderId = Cache.ChatterAuthorizationData!.UserId,
                Message = message,
                ReplyParentMessageId = replyParentMessageId,
            };
            return Utils.SendPostRequest(client, "https://api.twitch.tv/helix/chat/messages", body, JsonContext.Default.SendChatMessage, JsonContext.Default.SendChatMessageResponse, cancellationToken);
        }
    }
}
