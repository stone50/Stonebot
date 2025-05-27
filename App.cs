namespace Stonebot {
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Themes.Simple;
    using Avalonia.Threading;
    using System.Threading;
    using UI;

    internal class App : Application {
        public override void Initialize() => Styles.Add(new SimpleTheme());

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.Startup += (_, _) => FireStartup(CancellationToken.None);
                desktop.Exit += (_, _) => Shutdown(CancellationToken.None);
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void FireStartup(CancellationToken cancellationToken) => Task.Run(() => {
            Utils.TryElseConsoleError(Logger.Init);
            Utils.TryElseError(Config.Init);
            Utils.TryElseError(Logger.DeleteExcessFiles);
            Utils.TryElseError(() => Cache.Init(cancellationToken));
            Dispatcher.UIThread.Invoke(() => ((ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow as MainWindow)?.MainPanel.UpdateUsers());
        }, cancellationToken);

        private static void Shutdown(CancellationToken cancellationToken) {
            Utils.TryElseError(() => WebSocketClient.Close(cancellationToken));
            Utils.TryElseError(Cache.Save);
            Utils.TryElseError(Config.Save);
            Utils.TryElseError(Logger.Shutdown);
        }
    }
}
