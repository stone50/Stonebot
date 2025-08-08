namespace Stonebot {
    using Models.EventSubMessages;
    using Scripting;

    internal static class ChatMessageHandler {
        public static void HandleChatMessage(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            if (channelChatMessageEvent.ChatterId == Cache.ChatterAuthorizationData!.UserId) {
                return;
            }

            if (channelChatMessageEvent.Message.Text.StartsWith('!') && CommandManager.TryUseCommand(channelChatMessageEvent)) {
                return;
            }

            // TODO: handle chat message
            Logger.Debug(channelChatMessageEvent.ChatterLogin, channelChatMessageEvent.Message.Text);
        }
    }
}
