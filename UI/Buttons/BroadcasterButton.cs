namespace Stonebot.UI.Buttons {
    internal class BroadcasterButton(MainPanel mainPanel) : UserButton(mainPanel) {
        public override AuthorizationData? AuthorizationData => Cache.BroadcasterAuthorizationData;

        public override void Authorize(CancellationToken cancellationToken) => Cache.CreateBroadcasterAuthorizationData(cancellationToken);

        public override void ClearAuthorizationData() => Cache.ClearBroadcasterAuthorizationData();
    }
}
