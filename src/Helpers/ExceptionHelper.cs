namespace Stonebot.Helpers {
    using System;

    internal static class ExceptionHelper {
        public static void TryElseConsoleError(Action action) => TryElse(action, Console.Error.WriteLine);

        public static void TryElseError(Action action) => TryElse(action, (e) => Logger.Error(e));

        public static void TryElse(Action action, Action<Exception> errorAction) {
            try {
                action();
            } catch (OperationCanceledException) {
            } catch (Exception e) {
                try {
                    errorAction(e);
                } finally { }
            }
        }
    }
}
