namespace Stonebot.UI.Buttons {
    using Avalonia.Media;

    internal class BroadcasterButton(
        MainWindow mainWindow,
        IImmutableBrush defaultBrush,
        IImmutableBrush hoveredBrush,
        IImmutableBrush pressedBrush
    ) : UserButton(mainWindow, defaultBrush, hoveredBrush, pressedBrush) {
        public override AuthorizationData? AuthorizationData => Cache.BroadcasterAuthorizationData;

        public override void Authorize(CancellationToken cancellationToken) => Cache.CreateBroadcasterAuthorizationData(cancellationToken);

        public override void ClearAuthorizationData() => Cache.ClearBroadcasterAuthorizationData();
    }
}
