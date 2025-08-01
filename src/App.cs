namespace Stonebot {
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Themes.Simple;
    using Avalonia.Threading;
    using Resources;
    using System.Threading;
    using UI;

    internal class App : Application {
        public override void Initialize() => Styles.Add(new SimpleTheme());

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.Startup += (_, _) => Startup(CancellationToken.None);
                desktop.Exit += (_, _) => Shutdown(CancellationToken.None);
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Startup(CancellationToken cancellationToken) {
            var loggerInitTask = Utils.FireTryElseConsoleError(Logger.Init, cancellationToken);
            var configInitTask = Utils.FireTryElseErrorAfter(Config.Init, cancellationToken, loggerInitTask);
            var deleteExcessLogFilesTask = Utils.FireTryElseErrorAfter(Logger.DeleteExcessFiles, cancellationToken, configInitTask);
            var cacheInitTask = Utils.FireTryElseErrorAfter(() => Cache.Init(cancellationToken), cancellationToken, configInitTask);
            FireUpdateMainPanelAfter(mainPanel => mainPanel.UpdateUsers(), cancellationToken, cacheInitTask);
            var customDataInitTask = Utils.FireTryElseError(CustomData.Init, cancellationToken);
            var commandManagerInitTask = Utils.FireTryElseError(CommandManager.Init, cancellationToken);
            FireUpdateMainPanelAfter(mainPanel => mainPanel.UpdateInteractionGrid(), cancellationToken, commandManagerInitTask);
            var copyScriptsTypeHintsFileTask = Utils.FireTryElseError(CopyScriptsTypeHintsFile, cancellationToken);
        }

        private static void Shutdown(CancellationToken cancellationToken) {
            var webSocketClientCloseTask = Utils.FireTryElseError(() => WebSocketClient.Close(cancellationToken), cancellationToken);
            var customDataSaveTask = Utils.FireTryElseErrorAfter(CustomData.Save, cancellationToken, webSocketClientCloseTask);
            var commandManaderSaveTask = Utils.FireTryElseError(CommandManager.Save, cancellationToken);
            var cacheSaveTask = Utils.FireTryElseError(Cache.Save, cancellationToken);
            var configSaveTask = Utils.FireTryElseError(Config.Save, cancellationToken);
            Utils.TryElseErrorAfter(Logger.Shutdown, webSocketClientCloseTask, customDataSaveTask, commandManaderSaveTask, cacheSaveTask, configSaveTask);
        }

        private static void CopyScriptsTypeHintsFile() {
            _ = Directory.CreateDirectory(Constants.ScriptsTypeHintsPackagePath);
            File.WriteAllText(Constants.ScriptsTypeHintsFilePath, Embedded.ScriptsTypeHintsFile);
        }

        private void FireUpdateMainPanelAfter(Action<MainPanel> update, CancellationToken cancellationToken, params Task[] tasks) => Utils.FireDoAfter(() => Dispatcher.UIThread.Invoke(() => update(((MainWindow)((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!).MainWindow!).MainPanel)), cancellationToken, tasks);
    }
}
