namespace Stonebot {
    using Avalonia;

    internal class Program {
        private static void Main(string[] args) => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToDelegate((log) => Logger.Warn(log))
            .StartWithClassicDesktopLifetime(args);
    }
}
