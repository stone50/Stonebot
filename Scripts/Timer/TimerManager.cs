namespace Stonebot.Scripts.Timer {
    using System.Linq;

    internal static class TimerManager {
        public static Timer[] Timers = new Timer[] {
            new("quote", UseActions.Quote, 1800)
        };

        public static Timer? GetTimer(string keyword) => Timers.FirstOrDefault(timer => timer.Keyword == keyword);
    }
}
