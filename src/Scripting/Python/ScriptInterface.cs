namespace Stonebot.Scripting.Python {
    using Models.EventSubMessages;
    using System.Diagnostics.CodeAnalysis;
    using Twitch;

#pragma warning disable IDE1006 // Naming Styles
    public class ScriptInterface(EventSubNotificationMessagePayloadEvent channelChatMessageEvent, UserPermission.Level permissionLevel) {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScriptInterface))]
        public readonly MessageData message_data = new(channelChatMessageEvent);
        public readonly ChatterPermission chatter_permission = new(permissionLevel);

        public static void log(params object?[] messages) => Logger.Info(messages);

        public static void log_warn(params object?[] messages) => Logger.Warn(messages);

        public static void log_error(params object?[] messages) => Logger.Error(messages);

        public static ChatResponse chat(string message, string? reply_parent_message_id = null) => new(Chat.Send(message, reply_parent_message_id));

        public ChatResponse reply(string message) => chat(message, message_data.message_id);
    }
#pragma warning restore IDE1006 // Naming Styles
}
