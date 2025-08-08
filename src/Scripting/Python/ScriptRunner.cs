namespace Stonebot.Scripting.Python {
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;
    using Models.EventSubMessages;

    internal static class ScriptRunner {
        public static readonly ScriptEngine Engine = Python.CreateEngine();

        public static void Run(ScriptSource script, EventSubNotificationMessagePayloadEvent channelChatMessageEvent, UserPermission.Level permissionLevel) {
            scope.SetVariable("Stonebot", new ScriptInterface(channelChatMessageEvent, permissionLevel));
            Utils.TryElseError(() => script.Execute(scope));
        }

        private static readonly ScriptScope scope = Engine.CreateScope();
    }
}
