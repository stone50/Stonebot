namespace Stonebot.UI.Buttons {
    internal class ToggleButton : SButtonBase {
        public bool State { get; protected set; }

        public ToggleButton(bool initialState) => SetState(initialState);

        public void SetState(bool state) {
            State = state;
            UpdateBackground();
        }

        protected override void OnClick() {
            SetState(!State);

            base.OnClick();
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

    }
}
