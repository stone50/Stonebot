namespace Stonebot.UI.CustomControls {
    using Avalonia.Controls;

    internal class Swappable : Panel {
        public Swappable() { }

        public Swappable(Control control1, Control control2) => Init(control1, control2);

        public void Init(Control control1, Control control2) {
            if (this.control1 is not null) {
                return;
            }

            if (this.control2 is not null) {
                return;
            }

            this.control1 = control1;
            this.control2 = control2;
            this.control1.IsVisible = true;
            this.control2.IsVisible = false;
            Children.Add(control1);
            Children.Add(control2);
        }

        public void Swap() {
            control1!.IsVisible = !control1.IsVisible;
            control2!.IsVisible = !control2.IsVisible;
        }

        private Control? control1;
        private Control? control2;
    }
}
