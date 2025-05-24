namespace Stonebot.UI {
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Threading;
    using System.Threading;

    internal class App : Application {
        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.Startup += (_, _) => FireStartup(CancellationToken.None);
                desktop.Exit += (_, _) => Shutdown(CancellationToken.None);
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void FireStartup(CancellationToken cancellationToken) => Task.Run(() => {
            TryElseConsoleError(Logger.Init);
            TryElseWarn(Config.Init);
            TryElseWarn(Logger.DeleteExcessFiles);
            TryElseWarn(() => Cache.Init(cancellationToken));
            Dispatcher.UIThread.Invoke(() => ((ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow as MainWindow)?.UpdateUsers());
        }, cancellationToken);

        private static void Shutdown(CancellationToken cancellationToken) {
            TryElseWarn(() => WebSocketClient.Close(cancellationToken));
            TryElseWarn(Cache.Save);
            TryElseWarn(Config.Save);
            TryElseWarn(Logger.Shutdown);
        }

        private static void TryElseConsoleError(Action action) => TryElseLog(action, Console.Error.WriteLine);

        private static void TryElseWarn(Action action) => TryElseLog(action, (e) => Logger.Warn(e));

        private static void TryElseLog(Action action, Action<Exception> log) {
            try {
                action();
            } catch (Exception e) {
                try {
                    log(e);
                } finally { }
            }
        }
    }
}
