namespace Stonebot.UI.Buttons.Links {
    using System.Diagnostics;

    internal class UrlLink(string contentText) : LinkBase(contentText) {
        protected override ProcessStartInfo GetProcessStartInfo() => new() {
            FileName = ContentText,
            UseShellExecute = true
        };
    }
}
