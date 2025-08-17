namespace Stonebot.UI.CustomControls.Buttons {
    using Avalonia.Interactivity;

    internal class ToggleButton : SButtonBase {
        public bool State { get; protected set; }

        public ToggleButton(bool initialState) {
            State = initialState;
            Click += OnClick;
        }

        protected override void UpdateBackground() => Background =
            State
                ? IsPressed
                    ? MainTheme.SuccessBrush3
                    : IsPointerOver
                        ? MainTheme.SuccessBrush1
                        : MainTheme.SuccessBrush2
                : IsPressed
                    ? MainTheme.DangerBrush3
                    : IsPointerOver
                        ? MainTheme.DangerBrush1
                        : MainTheme.DangerBrush2;

        private void OnClick(object? sender, RoutedEventArgs e) {
            State = !State;
            UpdateBackground();
        }
    }
}
