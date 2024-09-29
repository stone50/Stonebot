namespace Stonebot.Scripts.Timer {
    using System;
    using System.Threading.Tasks;

    internal class Timer {
        public event EventHandler<bool> IsEnabledChanged = delegate { };
        public event EventHandler<int> IntervalChanged = delegate { };

        public string Keyword;
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }
        public int Interval { get => interval; set => SetInterval(value); }
        public DateTime LastTimeout { get; private set; }
        public Func<Task> TimeoutAction;

        public Timer(string keyword, Func<Task> timeoutAction, int interval) {
            Keyword = keyword;
            TimeoutAction = timeoutAction;
            LastTimeout = DateTime.Now;
            Interval = interval;
            Start();
        }

        public void SetIsEnabled(bool isEnabled) {
            Logger.Info($"Setting is enabled of timer {Keyword}.");
            if (this.isEnabled == isEnabled) {
                return;
            }

            this.isEnabled = isEnabled;
            if (IsEnabled) {
                Start();
            }

            Util.InvokeDeferred(IsEnabledChanged, IsEnabled);
        }

        public void SetInterval(int interval) {
            Logger.Info($"Setting interval of timer {Keyword}.");
            this.interval = interval;
            Util.InvokeDeferred(IntervalChanged, Interval);
        }

        private bool isEnabled = true;
        private int interval;

        private void Start() => Task.Run(async () => {
            while (IsEnabled) {
                if (DateTime.Now < LastTimeout.AddSeconds(Interval)) {
                    continue;
                }

                LastTimeout = DateTime.Now;
                await TimeoutAction();
            }
        });
    }
}
