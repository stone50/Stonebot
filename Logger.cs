namespace Stonebot {
    using System;
    using System.Diagnostics;
    using System.Text;

    internal static class Logger {
        public enum LogType {
            Debug,
            Info,
            Warning,
            Error
        }

        [Conditional("DEBUG")]
        public static void Debug(params object?[] messages) => Log(LogType.Debug, messages);

        public static void Info(params object?[] messages) => Log(LogType.Info, messages);

        public static void Warn(params object?[] messages) => Log(LogType.Warning, messages);

        public static void Error(params object?[] messages) => Log(LogType.Error, messages);

        public static void Log(LogType logType, params object?[] messages) => logQueue.Enqueue($"[{GetFormattedDateTime()}] {logType.ToString().ToUpper()}: {string.Join(" | ", messages)}");

        public static void Init() {
            try {
                _ = Directory.CreateDirectory(Constants.LogsPath);
                filePath = Path.Join(Constants.LogsPath, $"{GetFormattedDateTime()}.txt");
                File.Create(filePath).Close();
            } catch (Exception e) {
                Warn(e);
            }

            flushingTask = new Task(FlushingTaskAction, flushingTaskCancellationTokenSource.Token);
            flushingTask.Start();
        }

        public static void DeleteExcessFiles() {
            var filePaths = Directory.GetFiles(Constants.LogsPath);
            if (filePaths.Length < Config.NumMaxLogFiles) {
                return;
            }

            var fileCreationTimes = new FileCreationTime[filePaths.Length];
            for (var i = 0; i < filePaths.Length; ++i) {
                fileCreationTimes[i] = new FileCreationTime(filePaths[i], File.GetCreationTimeUtc(filePaths[i]));
            }

            foreach (var fileCreationTime in fileCreationTimes.OrderBy(fileCreationTime => fileCreationTime.CreationTime).Take(filePaths.Length - Config.NumMaxLogFiles)) {
                File.Delete(fileCreationTime.FilePath);
            }
        }

        public static async Task ShutdownAsync() {
            await flushingTaskCancellationTokenSource.CancelAsync().ConfigureAwait(false);
            if (flushingTask is not null) {
                await flushingTask.ConfigureAwait(false);
                flushingTask.Dispose();
            }

            flushingTaskCancellationTokenSource.Dispose();
            FlushQueue();
        }

        private readonly struct FileCreationTime(string filePath, DateTime creationTime) {
            public readonly string FilePath = filePath;
            public readonly DateTime CreationTime = creationTime;
        }

        private static readonly Queue<string> logQueue = new();
        private static string? filePath;
        private static Task? flushingTask;
        private static readonly CancellationTokenSource flushingTaskCancellationTokenSource = new();

        private static string GetFormattedDateTime() => DateTime.UtcNow.ToString("dd-MM-yyyy_HH.mm.ss.fff");

        private static void FlushingTaskAction() {
            while (!flushingTaskCancellationTokenSource.IsCancellationRequested) {
                try {
                    FlushQueue();
                } catch (OperationCanceledException) {
                    return;
                } catch (Exception e) {
                    Error(e);
                    return;
                }
            }
        }

        private static void FlushQueue() {
            var numLogs = logQueue.Count;
            if (numLogs == 0) {
                return;
            }

            var logs = new StringBuilder();
            for (var i = 0; i < numLogs; ++i) {
                var log = logQueue.Dequeue();
                WriteToConsole(log);
                if (filePath is not null) {
                    _ = logs.AppendLine(log);
                }
            }

            if (filePath is not null) {
                File.AppendAllText(filePath, logs.ToString());
            }
        }

        [Conditional("DEBUG")]
        private static void WriteToConsole(string log) => Console.WriteLine(log);
    }
}