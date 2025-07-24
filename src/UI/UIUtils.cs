namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using System;

    internal static class UIUtils {
        public static Image GetLogo() {
            var uri = new Uri(Constants.LogoAvaloniaResourceFilePath);
            var assetStream = AssetLoader.Open(uri);
            var bitmap = new Bitmap(assetStream);
            var logo = new Image {
                Source = bitmap,
                Stretch = Stretch.UniformToFill,
            };
            RenderOptions.SetBitmapInterpolationMode(logo, BitmapInterpolationMode.None);
            return logo;
        }

        public static Image GetConfigIcon() {
            var uri = new Uri(Constants.CogAvaloniaResourceFilePath);
            var assetStream = AssetLoader.Open(uri);
            var bitmap = new Bitmap(assetStream);
            var icon = new Image {
                Source = bitmap,
                Stretch = Stretch.UniformToFill,
            };
            return icon;
        }
    }
}
