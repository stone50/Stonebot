namespace Stonebot.Scripts.UI {
    using Godot;
    using Timer = Timer.Timer;

    internal partial class TimerPanel : Control {
        public void Init(Timer timer) {
            Timer = timer;

            KeywordLabel.Text = Timer.Keyword;
            MainButton.Pressed += OnMainButtonPressed;

            IntervalSpinBox.Value = Timer.Interval;
            IntervalSpinBox.ValueChanged += OnIntervalSpinBoxValueChanged;

            Timer.IntervalChanged += OnIntervalChanged;

            MainButton.Modulate = Timer.IsEnabled ? Colors.White : Colors.Red;
            EnableButton.Icon = Timer.IsEnabled ? Resources.EnableIcon : Resources.DisableIcon;
            EnableButton.Pressed += OnEnableButtonPressed;

            Timer.IsEnabledChanged += OnIsEnabledChanged;

            EnableButton.MouseEntered += OnMouseEnteredEnableButton;
            EnableButton.MouseExited += OnMouseExitedEnableButton;
        }

        [Export]
        private Label KeywordLabel = null!;
        [Export]
        private Button MainButton = null!;
        [Export]
        private Container DetailsContainer = null!;
        [Export]
        private SpinBox IntervalSpinBox = null!;
        [Export]
        private Button EnableButton = null!;

        private Timer Timer = null!;

        private bool IsHovering = false;

        private void OnMainButtonPressed() => DetailsContainer.Visible = !DetailsContainer.Visible;

        private void OnIntervalSpinBoxValueChanged(double value) => Timer.Interval = (int)value;

        private void OnIntervalChanged(object? _, int useDelay) => Util.CallDeferred(() => IntervalSpinBox.Value = useDelay);

        private void OnEnableButtonPressed() => Timer.IsEnabled = !Timer.IsEnabled;

        private void OnIsEnabledChanged(object? _, bool isEnabled) {
            MainButton.Modulate = isEnabled ? Colors.White : Colors.Red;
            UpdateIcon();
        }

        private void OnMouseEnteredEnableButton() {
            IsHovering = true;
            UpdateIcon();
        }

        private void OnMouseExitedEnableButton() {
            IsHovering = false;
            UpdateIcon();
        }

        private void UpdateIcon() => EnableButton.Icon = Timer.IsEnabled ^ IsHovering ? Resources.EnableIcon : Resources.DisableIcon;
    }
}
