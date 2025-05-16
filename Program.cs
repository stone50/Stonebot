namespace Stonebot {
    using Avalonia;
    using UI;

    internal class Program {
        private static void Main(string[] args) => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToDelegate((log) => Logger.Warn(log))
            .StartWithClassicDesktopLifetime(args);
    }
}
