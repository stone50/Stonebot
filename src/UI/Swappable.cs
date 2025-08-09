namespace Stonebot.UI {
    using Avalonia.Controls;

    internal class Swappable : Panel {
        public Swappable(Control control1, Control control2) {
            this.control1 = control1;
            this.control2 = control2;
            this.control1.IsVisible = true;
            this.control2.IsVisible = false;
            Children.Add(control1);
            Children.Add(control2);
        }

        public void Swap() {
            control1.IsVisible = !control1.IsVisible;
            control2.IsVisible = !control2.IsVisible;
        }

        private readonly Control control1;
        private readonly Control control2;
    }
}
