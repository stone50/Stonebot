namespace StoneBot.Scripts {
    using Godot;
    using System;

    internal static class Util {
        public static void InvokeDeferred<T>(EventHandler<T> eventHandler, T args) => Callable.From(() => eventHandler.Invoke(null, args)).CallDeferred();
    }
}