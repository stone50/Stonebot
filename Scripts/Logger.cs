namespace StoneBot.Scripts {
    using Godot;
    using System;

    internal static class Logger {
        public static event EventHandler<string> DebugLogged = delegate { };
        public static event EventHandler<string> InfoLogged = delegate { };
        public static event EventHandler<string> WarningLogged = delegate { };
        public static event EventHandler<string> ErrorLogged = delegate { };

        public static void Debug(string message) => HandleMessage("DEBUG", message, DebugLogged);

        public static void Info(string message) => HandleMessage("INFO", message, InfoLogged);

        public static void Warning(string message) => HandleMessage("WARNING", message, WarningLogged);

        public static void Error(string message) => HandleMessage("ERROR", message, ErrorLogged);

        private static void HandleMessage(string messageType, string message, EventHandler<string> eventHandler) {
            GD.Print($"[{DateTime.Now}] {messageType}: {message}");
            Util.InvokeDeferred(eventHandler, message);
        }
    }
}