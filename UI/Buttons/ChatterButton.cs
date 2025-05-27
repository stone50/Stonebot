namespace Stonebot.UI.Buttons {
    internal class ChatterButton(MainPanel mainPanel) : UserButton(mainPanel) {
        public override AuthorizationData? AuthorizationData => Cache.ChatterAuthorizationData;

        public override void Authorize(CancellationToken cancellationToken) => Cache.CreateChatterAccessToken(cancellationToken);

        public override void ClearAuthorizationData() => Cache.ClearChatterAuthorizationData();
    }
}
