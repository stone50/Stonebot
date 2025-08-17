namespace Stonebot.UI.CustomControls.Buttons.Links {
    using System.Diagnostics;

    internal class UrlLink(string contentText) : LinkBase(contentText) {
        protected override ProcessStartInfo GetProcessStartInfo() => new() {
            FileName = TextBlock.Text,
            UseShellExecute = true,
        };
    }
}
