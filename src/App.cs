namespace Stonebot {
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Themes.Simple;
    using Avalonia.Threading;
    using Resources;
    using Scripting;
    using UI;

    internal class App : Application {
        public override void Initialize() => Styles.Add(new SimpleTheme());

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.Startup += (_, _) => Startup();
                desktop.Exit += (_, _) => Shutdown();
                Utils.TryElseConsoleError(() => desktop.MainWindow = new MainWindow());
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Startup() {
            var loggerInitTask = Utils.FireTryElseConsoleError(Logger.Init);
            var configInitTask = Utils.FireTryElseErrorAfter(Config.Init, loggerInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.InitConfigPanel(), configInitTask);
            var deleteExcessLogFilesTask = Utils.FireTryElseErrorAfter(Logger.DeleteExcessFiles, configInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.UpdateMainPanelUserButtons(), configInitTask);
            var customDataInitTask = Utils.FireTryElseErrorAfter(CustomData.Init, loggerInitTask);
            var commandManagerInitTask = Utils.FireTryElseErrorAfter(CommandManager.Init, loggerInitTask);
            FireUpdateMainWindowAfter(mainWindow => mainWindow.UpdateMainPanelInteractionGrid(), commandManagerInitTask);
            var copyScriptsTypeHintsFileTask = Utils.FireTryElseErrorAfter(CopyScriptsTypeHintsFile, loggerInitTask);
            var scriptFilesWatcherInitTask = Utils.FireTryElseErrorAfter(ScriptFilesWatcher.Init, commandManagerInitTask);
        }

        private static void Shutdown() {
            var webSocketClientCloseTask = Utils.FireTryElseError(WebSocketClient.Close);
            var customDataSaveTask = Utils.FireTryElseErrorAfter(CustomData.Save, webSocketClientCloseTask);
            var commandManaderSaveTask = Utils.FireTryElseError(CommandManager.Save);
            var configSaveTask = Utils.FireTryElseError(Config.Save);
            Utils.TryElseErrorAfter(Logger.Shutdown, webSocketClientCloseTask, customDataSaveTask, commandManaderSaveTask, configSaveTask);
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
            void tryElseErrorUpdateOnUIThread() => Utils.TryElseError(updateOnUIThread);
            _ = Utils.FireDoAfter(tryElseErrorUpdateOnUIThread, tasks);
        }
    }
}
