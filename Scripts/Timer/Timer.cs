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
        public Func<Task<bool>> TimeoutAction;

        public Timer(string keyword, Func<Task<bool>> timeoutAction, int interval) {
            Logger.Info($"{nameof(Timer)} | Constructor\n{nameof(keyword)}: {keyword}\n{nameof(interval)}: {interval}");

            Keyword = keyword;
            TimeoutAction = timeoutAction;
            LastTimeout = DateTime.Now;
            Interval = interval;
            Start();
        }

        public void SetIsEnabled(bool isEnabled) {
            Logger.Info($"{nameof(Timer)} | {nameof(SetIsEnabled)}\n{nameof(isEnabled)}: {isEnabled}");

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
            Logger.Info($"{nameof(Timer)} | {nameof(SetInterval)}\n{nameof(interval)}: {interval}");

            this.interval = interval;
            Util.InvokeDeferred(IntervalChanged, Interval);
        }

        private bool isEnabled = true;
        private int interval;

        private void Start() {
            Logger.Info($"{nameof(Timer)} | {nameof(Start)}");

            _ = Task.Run(async () => {
                while (IsEnabled) {
                    if (DateTime.Now < LastTimeout.AddSeconds(Interval)) {
                        continue;
                    }

                    LastTimeout = DateTime.Now;
                    if (!await TimeoutAction()) {
                        Logger.Warning($"{nameof(Timer)} | ProcLoop | {nameof(TimeoutAction)} result is false.");
                        return;
                    }
                }
            });
        }
    }
}
