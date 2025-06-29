namespace Stonebot.UI.Buttons.Links {
    using System.Diagnostics;

    internal class FolderLink(string contentText, string path) : LinkBase(contentText) {
        public string Path = path;

        protected override ProcessStartInfo GetProcessStartInfo() => new() {
            FileName = "explorer.exe",
            Arguments = Path
        };
    }
}
