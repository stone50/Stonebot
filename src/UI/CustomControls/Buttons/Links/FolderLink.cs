namespace Stonebot.UI.CustomControls.Buttons.Links {
    using System.Diagnostics;

    internal class FolderLink(string path) : LinkBase() {
        public string Path = path;

        protected override ProcessStartInfo GetProcessStartInfo() => new() {
            Arguments = Path,
            FileName = "explorer.exe",
        };
    }
}
