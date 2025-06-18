namespace Stonebot {
    using Microsoft.Scripting.Hosting;
    using Models.EventSubMessages;
    using System;
    using System.Collections.Generic;

    internal class Command {
        public string Name;
        public List<string> Aliases;
        // TODO: add tags, add permissions
        public int CooldownMillis;

        public Command(string name, string[] aliases, int cooldownMillis) {
            Name = name;
            Aliases = [.. aliases];
            CooldownMillis = cooldownMillis;
            lastProcTime = DateTime.UtcNow.AddMilliseconds(-CooldownMillis);
            var scriptFilePath = Path.Join(Constants.CommandScriptsPath, $"{Name}.py");
            if (!File.Exists(scriptFilePath)) {
                File.Create(scriptFilePath).Close();
            }

            scriptSource = PythonRunner.Engine.CreateScriptSourceFromFile(scriptFilePath);
        }

        public void ReloadScriptFile() => scriptSource = PythonRunner.Engine.CreateScriptSourceFromFile(scriptSource.Path);

        public bool CanProc() => lastProcTime.AddMilliseconds(CooldownMillis) <= DateTime.UtcNow;

        public bool TryProc(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            if (!CanProc()) {
                return false;
            }

            lastProcTime = DateTime.UtcNow;
            PythonRunner.RunScript(scriptSource, channelChatMessageEvent);
            return true;
        }

        private DateTime lastProcTime;
        private ScriptSource scriptSource;
    }
}
