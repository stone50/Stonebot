namespace StoneBot.Scripts.Timer {
    using System;
    using System.Threading.Tasks;

    internal class Timer {
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }
        public int Interval;
        public DateTime LastTimeout { get; private set; }
        public Func<Task> TimeoutAction;

        public Timer(Func<Task> timeoutAction, int interval) {
            TimeoutAction = timeoutAction;
            LastTimeout = DateTime.Now;
            Interval = interval;
            Start();
        }

        public void SetIsEnabled(bool isEnabled) {
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
