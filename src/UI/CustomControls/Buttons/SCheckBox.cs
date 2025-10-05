namespace Stonebot.UI.CustomControls.Buttons {
    using Avalonia.Controls;

    internal class SCheckBox : ToggleButton {
        public SCheckBox(bool initialState) : base(initialState) {
            checkIcon.IsVisible = State;

            BorderBrush = MainTheme.NeutralBrush3;
            BorderThickness = new(1d);
            Content = checkIcon;
        }

        public override void Toggle() {
            base.Toggle();
            checkIcon.IsVisible = State;
        }

        protected override void UpdateBackground() => Background =
            IsPressed
                ? MainTheme.NeutralBrush6
                : IsPointerOver
                    ? MainTheme.NeutralBrush4
                    : MainTheme.NeutralBrush5;

        private readonly Image checkIcon = UIUtils.GetCheckIcon();
    }
}
