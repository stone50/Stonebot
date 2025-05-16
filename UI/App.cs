namespace Stonebot.UI {
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using System.Threading;

    internal class App : Application {
        public override void Initialize() { }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.Startup += async (_, args) => {
                    await StartupAsync();
                    if (desktop.MainWindow is MainWindow mainWindow) {
                        mainWindow.UpdateUsers();
                    }
                };

                desktop.Exit += async (_, args) => args.ApplicationExitCode = await ShutdownAsync().ConfigureAwait(false);

                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static async Task StartupAsync() {
            try {
                Logger.Init();
            } catch {
                Utils.Exit(Constants.ExitCode.LoggerInitError);
            }

            await Config.InitAsync(CancellationToken.None).ConfigureAwait(false);
            try {
                Logger.DeleteExcessFiles();
            } catch (Exception e) {
                Logger.Error(e);
                try {
                    await Logger.ShutdownAsync().ConfigureAwait(false);
                } catch {
                    Utils.Exit(Constants.ExitCode.LoggerDeleteExcessFilesError | Constants.ExitCode.LoggerShutdownError);
                }

                Utils.Exit(Constants.ExitCode.LoggerDeleteExcessFilesError);
            }

            await Cache.InitAsync(CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task<int> ShutdownAsync() {
            try {
                await Cache.SaveAsync(CancellationToken.None).ConfigureAwait(false);
            } catch (Exception e) {
                Logger.Warn(e);
            }

            Cache.ClearAll();
            try {
                await Config.SaveAsync(CancellationToken.None).ConfigureAwait(false);
            } catch (Exception e) {
                Logger.Warn(e);
            }

            try {
                await Logger.ShutdownAsync().ConfigureAwait(false);
            } catch {
                return (int)Constants.ExitCode.LoggerShutdownError;
            }

            return 0;
        }
    }
}
