namespace Stonebot.Scripting.Python {
    using IronPython.Hosting;
    using Microsoft.Scripting.Hosting;
    using Models.EventSubMessages;

    internal static class ScriptRunner {
        public static readonly ScriptEngine Engine = Python.CreateEngine();

        public static void Init() {
            var searchPaths = Engine.GetSearchPaths();
            searchPaths.Add(Constants.ScriptsPath);
            searchPaths.Add(Constants.ScriptsLibPath);
            Engine.SetSearchPaths(searchPaths);
        }

        public static void Run(ScriptSource script, EventSubNotificationMessagePayloadEvent channelChatMessageEvent, UserPermission.Level permissionLevel) {
            scope.SetVariable("Stonebot", new ScriptInterface(channelChatMessageEvent, permissionLevel));
            try {
                script.Execute(scope);
            } catch (Exception e) {
                Logger.Error(e);
            }
        }

        private static readonly ScriptScope scope = Engine.CreateScope();
    }
}
