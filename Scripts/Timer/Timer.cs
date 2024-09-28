namespace StoneBot.Scripts.Timer {
    using System;
    using System.Threading.Tasks;

    internal class Timer {
        public string Keyword;
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }
        public int Interval;
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
        }

        private bool isEnabled = true;

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
