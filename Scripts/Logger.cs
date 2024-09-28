namespace StoneBot.Scripts {
    using Godot;
    using System;

    internal static class Logger {
        public enum LogType {
            Debug,
            Info,
            Warning,
            Error
        }

        public struct MessageLoggedArgs {
            public string LogMessage;
            public LogType LogType;
        }

        public static event EventHandler<MessageLoggedArgs> MessageLogged = delegate { };

        public static void Debug(string message) => Log(LogType.Debug, message);
        public static void Debug(object obj) => Log(LogType.Debug, obj);

        public static void Info(string message) => Log(LogType.Info, message);
        public static void Info(object obj) => Log(LogType.Info, obj);

        public static void Warning(string message) => Log(LogType.Warning, message);
        public static void Warning(object obj) => Log(LogType.Warning, obj);

        public static void Error(string message) => Log(LogType.Error, message);
        public static void Error(object obj) => Log(LogType.Error, obj);

        public static void Log(LogType logType, string message) {
            var logMessage = $"[{DateTime.Now}] {logType.ToString().ToUpper()}: {message}";
            GD.Print(logMessage);
            Util.InvokeDeferred(MessageLogged, new() {
                LogMessage = logMessage,
                LogType = logType,
            });
        }
        public static void Log(LogType logType, object obj) => Log(logType, obj?.ToString() ?? "null");
    }
}