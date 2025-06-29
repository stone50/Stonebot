namespace Stonebot.Python {
    using Models.EventSubMessages;
    using System;
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

        public static ChatResponse chat(string message, string? reply_parent_message_id = null) {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.SendChatMessageFromScriptTimeoutSeconds));
            return new(Chat.Send(message, reply_parent_message_id, cancellationTokenSource.Token));
        }

        public static object? get_data(string key) => CustomData.Get(key);

        public static void set_data(string key, object value) {
            set_data_without_saving(key, value);
            save_data();
        }

        public static void set_data_without_saving(string key, object value) => CustomData.Set(key, value);

        public static bool delete_data(string key) {
            if (!delete_data_without_saving(key)) {
                return false;
            }

            save_data();
            return true;
        }

        public static bool delete_data_without_saving(string key) => CustomData.Delete(key);

        public static bool data_contains(string key) => CustomData.Contains(key);

        public static void save_data() => CustomData.Save();
    }
#pragma warning restore IDE1006 // Naming Styles
}
