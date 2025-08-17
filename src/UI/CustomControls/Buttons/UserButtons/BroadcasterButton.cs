namespace Stonebot.UI.CustomControls.Buttons.UserButtons {
    using Popups;

    internal class BroadcasterButton(RemoveAuthorizationPopup removeAuthorizationPopup, CancelPopup cancelAuthorizationPopup) : UserButton(removeAuthorizationPopup, cancelAuthorizationPopup) {
        protected override AuthorizationData? AuthorizationData => Cache.BroadcasterAuthorizationData;

        protected override void CreateAccessToken(CancellationToken cancellationToken) => Cache.CreateBroadcasterAuthorizationData(cancellationToken);

        protected override void ClearAuthorizationData() => Cache.ClearBroadcasterAuthorizationData();
    }
}
