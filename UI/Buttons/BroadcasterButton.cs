namespace Stonebot.UI.Buttons {
    using Avalonia.Media;

    internal class BroadcasterButton(RemoveAuthorizationPopup removeAuthorizationPopup, IImmutableBrush defaultBrush, IImmutableBrush hoveredBrush, IImmutableBrush pressedBrush) : UserButton(removeAuthorizationPopup, defaultBrush, hoveredBrush, pressedBrush) {
        public override AuthorizationData? AuthorizationData => Cache.BroadcasterAuthorizationData;

        public override void CreateAuthorizationData(CancellationToken cancellationToken) => Cache.CreateBroadcasterAuthorizationData(cancellationToken);

        public override void ClearAuthorizationData() => Cache.ClearBroadcasterAuthorizationData();
    }
}
