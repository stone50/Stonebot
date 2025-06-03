namespace Stonebot {
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;
    using Models.EventSubMessages;
    using System;
    using Twitch;

    internal static class PythonRunner {
        public static readonly ScriptEngine Engine = Python.CreateEngine();

        public static void RunScript(ScriptSource script, EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            scope.SetVariable("Stonebot", new ScriptInterface(channelChatMessageEvent));
            script.Execute(scope);
        }

        private static readonly ScriptScope scope = Engine.CreateScope();
    }

    public class ScriptInterface(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
        public readonly EventSubNotificationMessagePayloadEvent ChatMessageData = channelChatMessageEvent;

        public static void SendMessage(string message, string? replyParentMessageId = null) => Utils.TryElseError(() => {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.SendChatMessageFromScriptTimeoutSeconds));
            var response = Chat.Send(message, replyParentMessageId, cancellationTokenSource.Token);
            if (!response.Data[0].IsSent) {
                throw new Exception($"{response.DropReason.Code}: {response.DropReason.Message}.");
            }
        });

        public object? GetData(string key) {
            // TODO
            return null;
        }

        public void SetData(string key, object value) {
            // TODO
        }
    }
}
