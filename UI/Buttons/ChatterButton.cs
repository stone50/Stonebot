namespace Stonebot.UI.Buttons {
    using Avalonia.Media;

    internal class ChatterButton(AuthorizePopup authorizePopup,
        RemoveAuthorizationPopup removeAuthorizationPopup,
        IImmutableBrush defaultBrush,
        IImmutableBrush hoveredBrush,
        IImmutableBrush pressedBrush
    ) : UserButton(authorizePopup, removeAuthorizationPopup, defaultBrush, hoveredBrush, pressedBrush) {
        public override AuthorizationData? AuthorizationData => Cache.ChatterAuthorizationData;

        public override void Authorize(CancellationToken cancellationToken) => Cache.CreateChatterAccessToken(cancellationToken);

        public override void ClearAuthorizationData() => Cache.ClearChatterAuthorizationData();
    }
}
