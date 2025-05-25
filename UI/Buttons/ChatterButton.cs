namespace Stonebot.UI.Buttons {
    using Avalonia.Media;

    internal class ChatterButton(RemoveAuthorizationPopup removeAuthorizationPopup, IImmutableBrush defaultBrush, IImmutableBrush hoveredBrush, IImmutableBrush pressedBrush) : UserButton(removeAuthorizationPopup, defaultBrush, hoveredBrush, pressedBrush) {
        public override AuthorizationData? AuthorizationData => Cache.ChatterAuthorizationData;

        public override void CreateAuthorizationData(CancellationToken cancellationToken) => Cache.CreateChatterAccessToken(cancellationToken);

        public override void ClearAuthorizationData() => Cache.ClearChatterAuthorizationData();

    }
}
