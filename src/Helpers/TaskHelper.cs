namespace Stonebot.Helpers {
    using System;
    using System.Threading.Tasks;

    internal static class TaskHelper {
        public static CancellationToken GetDefaultCancellationToken() => GetCancellationTokenFromSeconds(Constants.DefaultCancellationTokenTimeoutSecs);

        public static CancellationToken GetCancellationTokenFromSeconds(long seconds) => new CancellationTokenSource(TimeSpan.FromSeconds(seconds)).Token;

        public static Task FireTryElseConsoleErrorAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => TryElseConsoleErrorAfter(action, tasks), cancellationToken);

        public static Task FireTryElseConsoleErrorAfter(Action action, params Task[] tasks) => Task.Run(() => TryElseConsoleErrorAfter(action, tasks));

        public static Task FireTryElseErrorAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => TryElseErrorAfter(action, tasks), cancellationToken);

        public static Task FireTryElseErrorAfter(Action action, params Task[] tasks) => Task.Run(() => TryElseErrorAfter(action, tasks));

        public static Task FireTryElseAfter(Action action, Action<Exception> errorAction, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => TryElseAfter(action, errorAction, tasks), cancellationToken);

        public static Task FireTryElseAfter(Action action, Action<Exception> errorAction, params Task[] tasks) => Task.Run(() => TryElseAfter(action, errorAction, tasks));

        public static void TryElseConsoleErrorAfter(Action action, params Task[] tasks) => ExceptionHelper.TryElseConsoleError(() => DoAfter(action, tasks));

        public static void TryElseErrorAfter(Action action, params Task[] tasks) => ExceptionHelper.TryElseError(() => DoAfter(action, tasks));

        public static void TryElseAfter(Action action, Action<Exception> errorAction, params Task[] tasks) => ExceptionHelper.TryElse(() => DoAfter(action, tasks), errorAction);

        public static Task FireDoAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => DoAfter(action, tasks), cancellationToken);

        public static Task FireDoAfter(Action action, params Task[] tasks) => Task.Run(() => DoAfter(action, tasks));

        public static void DoAfter(Action action, params Task[] tasks) {
            foreach (var task in tasks) {
                Sync(task);
            }

            action();
        }

        public static T Sync<T>(Task<T> task) => task.GetAwaiter().GetResult();

        public static T Sync<T>(ValueTask<T> task) => task.GetAwaiter().GetResult();

        public static void Sync(Task task) => task.GetAwaiter().GetResult();

        public static Task FireTryElseConsoleError(Action action, CancellationToken cancellationToken) => Task.Run(() => ExceptionHelper.TryElseConsoleError(action), cancellationToken);

        public static Task FireTryElseConsoleError(Action action) => Task.Run(() => ExceptionHelper.TryElseConsoleError(action));

        public static Task FireTryElseError(Action action, CancellationToken cancellationToken) => Task.Run(() => ExceptionHelper.TryElseError(action), cancellationToken);

        public static Task FireTryElseError(Action action) => Task.Run(() => ExceptionHelper.TryElseError(action));

        public static Task FireTryElse(Action action, Action<Exception> errorAction, CancellationToken cancellationToken) => Task.Run(() => ExceptionHelper.TryElse(action, errorAction), cancellationToken);

        public static Task FireTryElse(Action action, Action<Exception> errorAction) => Task.Run(() => ExceptionHelper.TryElse(action, errorAction));

    }
}
