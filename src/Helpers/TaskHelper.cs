namespace Stonebot.Helpers {
    using System;
    using System.Threading.Tasks;

    internal static class TaskHelper {
        public static CancellationToken GetDefaultCancellationToken() => GetCancellationTokenFromSeconds(Constants.DefaultCancellationTokenTimeoutSecs);

        public static CancellationToken GetCancellationTokenFromSeconds(long seconds) => new CancellationTokenSource(TimeSpan.FromSeconds(seconds)).Token;

        public static Task FireTryElseConsoleError(Action action) => Task.Run(() => {
            try {
                action();
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        });

        public static Task FireTryElseError(Action action) => Task.Run(() => {
            try {
                action();
            } catch (Exception e) {
                Logger.Error(e);
            }
        });

        public static Task FireTryElseErrorAfter(Action action, params Task[] tasks) => Task.Run(() => {
            foreach (var task in tasks) {
                Sync(task);
            }

            try {
                action();
            } catch (Exception e) {
                Logger.Error(e);
            }
        });

        public static T Sync<T>(Task<T> task) => task.GetAwaiter().GetResult();

        public static void Sync(Task task) => task.GetAwaiter().GetResult();
    }
}
