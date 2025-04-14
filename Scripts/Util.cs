namespace Stonebot.Scripts {
    using Godot;
    using System;
    using Environment = System.Environment;

    internal static class Util {
        public static void CallDeferred(Action action) => Callable.From(action).CallDeferred();

        public static void InvokeDeferred<T>(EventHandler<T> eventHandler, T args) => CallDeferred(() => eventHandler.Invoke(null, args));

        public static string GetMasked(string? value) => value is null ? "null" : value.Length <= 4 ? "xxxx" : $"...{value[^4..]}";

        public static string GetMaskedPath(string path) => path.Replace(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace('\\', '/'), "~");

    }
}