namespace Stonebot.Scripting {
    using Helpers;

    internal static class ScriptFilesWatcher {
        public static void Init() {
            fileSystemWatcher.Path = Constants.ScriptsPath;
            fileSystemWatcher.Changed += (_, e) => ReloadScript(e.FullPath);
            fileSystemWatcher.Error += (_, e) => Logger.Error(e.GetException());
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private static void ReloadScript(string scriptFilePath) {
            if (scriptFilePath.StartsWith(Constants.CommandScriptsPath)) {
                foreach (var command in CommandManager.Commands) {
                    if (command.GetScriptFilePath() == scriptFilePath) {
                        ExceptionHelper.TryElseError(command.ReloadScriptFile);
                        return;
                    }
                }

                return;
            }
        }

        private static readonly FileSystemWatcher fileSystemWatcher = new() {
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "*.py",
            IncludeSubdirectories = true,
        };
    }
}
