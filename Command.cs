namespace Stonebot {
    using Microsoft.Scripting.Hosting;
    using Models.EventSubMessages;
    using PythonInterface;
    using System;
    using System.Collections.Generic;

    internal class Command {
        public string Name;
        public List<string> Aliases;
        public bool Enabled;
        public UserPermission.Level PermissionLevel;
        public int CooldownMillis;

        public Command(string name, string[] aliases, bool enabled, UserPermission.Level permissionLevel, int cooldownMillis) {
            Name = name;
            Aliases = [.. aliases];
            Enabled = enabled;
            PermissionLevel = permissionLevel;
            CooldownMillis = cooldownMillis;
            lastProcTime = DateTime.UtcNow.AddMilliseconds(-CooldownMillis);
            var scriptFilePath = Path.Join(Constants.CommandScriptsPath, $"{Name}.py");
            if (!File.Exists(scriptFilePath)) {
                File.Create(scriptFilePath).Close();
            }

            scriptSource = ScriptRunner.Engine.CreateScriptSourceFromFile(scriptFilePath);
        }

        public void ReloadScriptFile() => scriptSource = ScriptRunner.Engine.CreateScriptSourceFromFile(scriptSource.Path);

        public bool TryProc(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            if (!Enabled) {
                return false;
            }

            if (lastProcTime.AddMilliseconds(CooldownMillis) > DateTime.UtcNow) {
                return false;
            }

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.GetHighestUserPermissionLevelTimeoutSeconds));
            var userPermissionLevel = UserPermission.GetHighestLevel(channelChatMessageEvent.ChatterId, cancellationTokenSource.Token);
            if (userPermissionLevel < PermissionLevel) {
                return false;
            }

            lastProcTime = DateTime.UtcNow;
            ScriptRunner.Run(scriptSource, channelChatMessageEvent, userPermissionLevel);
            return true;
        }

        private DateTime lastProcTime;
        private ScriptSource scriptSource;
    }
}
