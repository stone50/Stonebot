namespace Stonebot.UI.CustomControls.Buttons.Links {
    using System.Diagnostics;

    internal class FolderLink(string path) : LinkBase(path) {
        protected override ProcessStartInfo GetProcessStartInfo() => new() {
            Arguments = TextBlock.Text,
            FileName = "explorer.exe",
        };
    }
}
