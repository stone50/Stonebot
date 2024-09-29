namespace Stonebot.Scripts.UI {
    using Godot;
    using Timer;

    internal partial class TimersTab : Control {
        public override void _Ready() {
            foreach (var message in TimerManager.Timers) {
                var timerPanel = Resources.TimerPanelScene.Instantiate<TimerPanel>();
                timerPanel.Init(message);
                TimersContainer.AddChild(timerPanel);
            }
        }

        [Export]
        private Container TimersContainer = null!;
    }
}
