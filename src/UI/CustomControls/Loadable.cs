namespace Stonebot.UI.CustomControls {
    using Avalonia.Controls;

    internal class Loadable : Panel {
        public Loadable(Control loadedControl, Action onLoad) {
            this.onLoad = onLoad;
            this.loadedControl = loadedControl;
            Children.Add(new SSelectableTextBlock() {
                Text = "..."
            });
        }

        public void Load() {
            Children.Clear();
            Children.Add(loadedControl);
            onLoad();
        }

        private readonly Control loadedControl;
        private readonly Action onLoad;
    }
}
