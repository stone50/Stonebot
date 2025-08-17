namespace Stonebot.Twitch {
    using Models.Bodies;
    using Models.Responses;

    internal static class Chat {
        public static SendChatMessageResponse Send(string message, string? replyParentMessageId) {
            var body = new SendChatMessageBody {
                BroadcasterId = Cache.BroadcasterId,
                SenderId = Cache.ChatterId,
                Message = message,
                ReplyParentMessageId = replyParentMessageId,
            };
            return Utils.SendAuthorizedPostRequest("https://api.twitch.tv/helix/chat/messages", body, JsonContext.Default.SendChatMessageBody, JsonContext.Default.SendChatMessageResponse);
        }
    }
}
