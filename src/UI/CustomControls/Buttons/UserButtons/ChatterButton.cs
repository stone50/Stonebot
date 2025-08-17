namespace Stonebot.UI.CustomControls.Buttons.UserButtons {
    using Popups;

    internal class ChatterButton(RemoveAuthorizationPopup removeAuthorizationPopup, CancelPopup cancelAuthorizationPopup) : UserButton(removeAuthorizationPopup, cancelAuthorizationPopup) {
        protected override AuthorizationData? AuthorizationData => Cache.ChatterAuthorizationData;

        protected override void CreateAccessToken(CancellationToken cancellationToken) => Cache.CreateChatterAccessToken(cancellationToken);

        protected override void ClearAuthorizationData() => Cache.ClearChatterAuthorizationData();
    }
}
