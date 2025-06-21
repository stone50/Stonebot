namespace Stonebot {
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;
    using Models;
    using Models.EventSubMessages;
    using System;
    using Twitch;

    internal static class PythonRunner {
        public static readonly ScriptEngine Engine = Python.CreateEngine();

        public static void RunScript(ScriptSource script, EventSubNotificationMessagePayloadEvent channelChatMessageEvent, UserPermission.Level permissionLevel) {
            scope.SetVariable("Stonebot", new ScriptInterface(channelChatMessageEvent, permissionLevel));
            Utils.TryElseError(() => script.Execute(scope));
        }

        private static readonly ScriptScope scope = Engine.CreateScope();
    }

    public class ScriptInterface(EventSubNotificationMessagePayloadEvent channelChatMessageEvent, UserPermission.Level permissionLevel) {
        public readonly EventSubNotificationMessagePayloadEvent ChatMessageData = channelChatMessageEvent;
        public readonly UserPermission.Level ChatterPermissionLevel = permissionLevel;

        public static void Log(params object?[] messages) => Logger.Info(messages);

        public static void LogWarn(params object?[] messages) => Logger.Warn(messages);

        public static void LogError(params object?[] messages) => Logger.Error(messages);

        public static SendChatMessageResponse SendMessage(string message, string? replyParentMessageId = null) {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.SendChatMessageFromScriptTimeoutSeconds));
            return Chat.Send(message, replyParentMessageId, cancellationTokenSource.Token);
        }

        public static object? GetData(string key) => CustomData.Get(key);

        public static void SetData(string key, object value) {
            SetDataWithoutSaving(key, value);
            SaveData();
        }

        public static void SetDataWithoutSaving(string key, object value) => CustomData.Set(key, value);

        public static bool DeleteData(string key) {
            if (!DeleteDataWithoutSaving(key)) {
                return false;
            }

            SaveData();
            return true;
        }

        public static bool DeleteDataWithoutSaving(string key) => CustomData.Delete(key);

        public static bool DataContains(string key) => CustomData.Contains(key);

        public static void SaveData() => CustomData.Save();
    }
}
