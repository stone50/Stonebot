namespace StoneBot.Scripts {
    using Godot;
    using System;

    internal static class Util {
        public static void CallDeferred(Action action) => Callable.From(action).CallDeferred();

        public static void InvokeDeferred<T>(EventHandler<T> eventHandler, T args) => CallDeferred(() => eventHandler.Invoke(null, args));
    }
}