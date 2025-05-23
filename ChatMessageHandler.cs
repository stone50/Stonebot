namespace Stonebot {
    using Models.EventSubMessages;

    internal static class ChatMessageHandler {
        // TODO
        public static void HandleChatMessage(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) => Logger.Debug(channelChatMessageEvent.ChatterLogin, channelChatMessageEvent.Message.Text);
    }
}
