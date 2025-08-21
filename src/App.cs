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
                ExceptionHelper.TryElseConsoleError(() => desktop.MainWindow = new MainWindow());
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Startup() {
            var loggerInitTask = TaskHelper.FireTryElseConsoleError(Logger.Init);
            var configInitTask = TaskHelper.FireTryElseErrorAfter(Config.Init, loggerInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.InitConfigPanel(), configInitTask);
            var deleteExcessLogFilesTask = TaskHelper.FireTryElseErrorAfter(Logger.DeleteExcessFiles, configInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.UpdateMainPanelUserButtons(), configInitTask);
            var customDataInitTask = TaskHelper.FireTryElseErrorAfter(CustomData.Init, loggerInitTask);
            var commandManagerInitTask = TaskHelper.FireTryElseErrorAfter(CommandManager.Init, loggerInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.UpdateMainPanelInteractionGrid(), commandManagerInitTask);
            var copyScriptsTypeHintsFileTask = TaskHelper.FireTryElseErrorAfter(CopyScriptsTypeHintsFile, loggerInitTask);
            var scriptFilesWatcherInitTask = TaskHelper.FireTryElseErrorAfter(ScriptFilesWatcher.Init, commandManagerInitTask);
        }

        private static void Shutdown() {
            var webSocketClientCloseTask = TaskHelper.FireTryElseError(WebSocketClient.Close);
            var customDataSaveTask = TaskHelper.FireTryElseErrorAfter(CustomData.Save, webSocketClientCloseTask);
            var commandManaderSaveTask = TaskHelper.FireTryElseError(CommandManager.Save);
            var configSaveTask = TaskHelper.FireTryElseError(Config.Save);
            TaskHelper.TryElseErrorAfter(Logger.Shutdown, webSocketClientCloseTask, customDataSaveTask, commandManaderSaveTask, configSaveTask);
        }

        private static void CopyScriptsTypeHintsFile() {
            _ = Directory.CreateDirectory(Constants.ScriptsTypeHintsPackagePath);
            File.WriteAllText(Constants.ScriptsTypeHintsFilePath, Embedded.ScriptsTypeHintsPyi);
        }

        private void FireUpdateMainWindowAfter(Action<MainWindow> update, params Task[] tasks) {
            void updateMainPanel() {
                var desktopApplicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
                var mainWindow = (MainWindow)desktopApplicationLifetime.MainWindow!;
                update(mainWindow);
            }

            void updateOnUIThread() => Dispatcher.UIThread.Invoke(updateMainPanel);
            void tryElseErrorUpdateOnUIThread() => ExceptionHelper.TryElseError(updateOnUIThread);
            _ = TaskHelper.FireDoAfter(tryElseErrorUpdateOnUIThread, tasks);
        }
    }
}
