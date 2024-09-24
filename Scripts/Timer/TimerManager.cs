namespace StoneBot.Scripts.Timer {
    using System.Collections.Generic;

    internal static class TimerManager {
        public static Dictionary<string, Timer> Timers = new();

        public static void Init() => Timers.Add("quote", new(UseActions.Quote, 3));
    }
}
