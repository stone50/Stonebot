namespace Stonebot.Twitch {
    using Helpers;
    using Models.Bodies;
    using Models.Responses;

    internal static class Chat {
        public static PostChatMessageResponse Send(string message, string? replyParentMessageId) {
            var body = new PostChatMessageBody {
                BroadcasterId = Cache.BroadcasterId,
                SenderId = Cache.ChatterId,
                Message = message,
                ReplyParentMessageId = replyParentMessageId,
            };
            return HttpHelper.SendAuthorizedPostRequest("https://api.twitch.tv/helix/chat/messages", body, JsonContext.Default.PostChatMessageBody, JsonContext.Default.PostChatMessageResponse);
        }
    }
}
