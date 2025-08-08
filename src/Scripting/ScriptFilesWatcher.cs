namespace Stonebot.Scripting {
    internal static class ScriptFilesWatcher {
        public static void Init() {
            fileSystemWatcher = new(Constants.ScriptsPath) {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.py",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };
            fileSystemWatcher.Changed += (_, e) => ReloadScript(e.FullPath);
            fileSystemWatcher.Error += (_, e) => Logger.Error(e.GetException());
        }

        private static void ReloadScript(string scriptFilePath) {
            if (scriptFilePath.StartsWith(Constants.CommandScriptsPath)) {
                foreach (var command in CommandManager.Commands) {
                    if (command.GetScriptFilePath() == scriptFilePath) {
                        Utils.TryElseError(command.ReloadScriptFile);
                    }
                }

                return;
            }
        }

        private static FileSystemWatcher? fileSystemWatcher;
    }
}
