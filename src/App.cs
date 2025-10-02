namespace Stonebot {
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Themes.Simple;
    using Avalonia.Threading;
    using Helpers;
    using Resources;
    using Scripting;
    using UI;

    internal class App : Application {
        public override void Initialize() => Styles.Add(new SimpleTheme());

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.Startup += (_, _) => Startup();
                desktop.Exit += (_, _) => Shutdown();
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Startup() {
            var loggerInitTask = TaskHelper.FireTryElseConsoleError(Logger.Init);
            var configInitTask = TaskHelper.FireTryElseErrorAfter(Config.Init, loggerInitTask);
            var deleteExcessLogFilesTask = TaskHelper.FireTryElseErrorAfter(Logger.DeleteExcessFiles, configInitTask);
            var cacheInitTask = TaskHelper.FireTryElseErrorAfter(Cache.Init, configInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.LoadMainPanelAuth(), cacheInitTask);
            var commandManagerInitTask = TaskHelper.FireTryElseErrorAfter(CommandManager.Init, loggerInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.LoadMainPanelInteractionGrid(), commandManagerInitTask);
            var copyScriptsTypeHintsFileTask = TaskHelper.FireTryElseErrorAfter(CopyScriptsTypeHintsFile, loggerInitTask);
            var scriptFilesWatcherInitTask = TaskHelper.FireTryElseErrorAfter(ScriptFilesWatcher.Init, commandManagerInitTask);
            TaskHelper.Sync(configInitTask);
            var desktopApplicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            var mainWindow = (MainWindow)desktopApplicationLifetime.MainWindow!;
            mainWindow.InitConfigPanel();
        }

        private static void Shutdown() {
            var webSocketClientCloseTask = WebSocketClient.Id == null ? Task.CompletedTask : TaskHelper.FireTryElseError(WebSocketClient.Close);
            var commandManaderSaveTask = TaskHelper.FireTryElseError(CommandManager.Save);
            var configSaveTask = TaskHelper.FireTryElseError(Config.Save);
        }

        private static void CopyScriptsTypeHintsFile() {
            _ = Directory.CreateDirectory(Constants.ScriptsTypeHintsPackagePath);
            File.WriteAllText(Constants.ScriptsTypeHintsFilePath, Embedded.ScriptsTypeHintsPyi);
        }

        private void FireUpdateMainWindowAfter(Action<MainWindow> update, params Task[] tasks) => TaskHelper.FireTryElseErrorAfter(() => Dispatcher.UIThread.Invoke(() => {
            var desktopApplicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            var mainWindow = (MainWindow)desktopApplicationLifetime.MainWindow!;
            update(mainWindow);
        }), tasks);
    }
}
