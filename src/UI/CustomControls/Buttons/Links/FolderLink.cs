namespace Stonebot.UI.CustomControls.Buttons.Links {
    using System.Diagnostics;

    internal class FolderLink : LinkBase {
        public string Path;

        public FolderLink(string path) : base() => Path = path;

        public FolderLink(string text, string path) : base(text) => Path = path;

        protected override ProcessStartInfo GetProcessStartInfo() => new() {
            Arguments = Path,
            FileName = "explorer.exe",
        };
    }
}
