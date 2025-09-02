namespace Stonebot {
    using Models.EventSubMessages;
    using Scripting;

    internal static class ChatMessageHandler {
        public static void HandleChatMessage(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            if (channelChatMessageEvent.ChatterId == Cache.GetChatterId()) {
                return;
            }

            if (channelChatMessageEvent.Message.Text.StartsWith('!') && CommandManager.TryUseCommand(channelChatMessageEvent)) {
                return;
            }

            // TODO: handle chat message
            Logger.Debug(channelChatMessageEvent.ChatterUserName, channelChatMessageEvent.Message.Text);
        }
    }
}
