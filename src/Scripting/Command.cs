namespace Stonebot.Scripting {
    using Microsoft.Scripting.Hosting;
    using Models.EventSubMessages;
    using Python;
    using Resources;
    using System;
    using System.Collections.Generic;

    internal class Command {
        public string Name;
        public List<string> Aliases;
        public bool Enabled;
        public UserPermission.Level PermissionLevel;
        public int CooldownSeconds;

        public Command(string name, string[] aliases, bool enabled, UserPermission.Level permissionLevel, int cooldownSeconds) {
            Name = name;
            Aliases = [.. aliases];
            Enabled = enabled;
            PermissionLevel = permissionLevel;
            CooldownSeconds = cooldownSeconds;
            lastProcTime = DateTime.UtcNow.AddSeconds(-CooldownSeconds);
            scriptSource = GetReloadedScriptSource();
        }

        public string GetScriptFilePath() => Path.Join(Constants.CommandScriptsPath, $"{Name}.py");

        public void ReloadScriptFile() => scriptSource = GetReloadedScriptSource();

        public bool TryProc(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            if (!Enabled) {
                return false;
            }

            if (lastProcTime.AddSeconds(CooldownSeconds) > DateTime.UtcNow) {
                return false;
            }

            var userPermissionLevel = UserPermission.GetHighestLevel(channelChatMessageEvent.Badges);
            if (userPermissionLevel < PermissionLevel) {
                return false;
            }

            lastProcTime = DateTime.UtcNow;
            ScriptRunner.Run(scriptSource, channelChatMessageEvent, userPermissionLevel);
            return true;
        }

        private DateTime lastProcTime;
        private ScriptSource scriptSource;

        private ScriptSource GetReloadedScriptSource() {
            var scriptFilePath = GetScriptFilePath();
            if (!File.Exists(scriptFilePath)) {
                File.WriteAllText(scriptFilePath, Embedded.ScriptsTemplatePy);
            }

            return ScriptRunner.Engine.CreateScriptSourceFromFile(scriptFilePath);
        }
    }
}
