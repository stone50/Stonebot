namespace Stonebot {
    using System;
    using System.Diagnostics;

    internal static class Logger {
        public enum LogType {
            Debug,
            Info,
            Warning,
            Error
        }

        public static void Init() {
            _ = Directory.CreateDirectory(Constants.LogsPath);
            filePath = Path.Join(Constants.LogsPath, $"{GetFormattedDateTime()}.txt");
            File.Create(filePath).Close();
        }

        [Conditional("DEBUG")]
        public static void Debug(params object?[]? messages) => Log(LogType.Debug, messages);

        public static void Info(params object?[]? messages) => Log(LogType.Info, messages);

        public static void Warn(params object?[]? messages) => Log(LogType.Warning, messages);

        public static void Error(params object?[]? messages) => Log(LogType.Error, messages);

        public static void Log(LogType logType, params object?[]? messages) {
            var log = $"[{GetFormattedDateTime()}] {logType.ToString().ToUpper()}: {string.Join(" | ", messages ?? [])}";
            LogToConsole(log);
            try {
                File.AppendAllText(filePath!, log);
            } catch (Exception e) {
                Console.Error.WriteLine($"Error logging \"{log}\": {e}");
            }
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

        private readonly struct FileCreationTime(string filePath, DateTime creationTime) {
            public readonly string FilePath = filePath;
            public readonly DateTime CreationTime = creationTime;
        }

        private static string? filePath;

        private static string GetFormattedDateTime() => DateTime.UtcNow.ToString("dd-MM-yyyy_HH.mm.ss.fff");

        [Conditional("CONSOLE")]
        private static void LogToConsole(string log) => Console.WriteLine(log);
    }
}