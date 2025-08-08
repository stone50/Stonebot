namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using System;

    internal static class UIUtils {
        public static Image GetLogo() {
            var logo = GetImage(Constants.LogoAvaloniaResourceFilePath);
            RenderOptions.SetBitmapInterpolationMode(logo, BitmapInterpolationMode.None);
            return logo;
        }

        public static Image GetConfigIcon() => GetImage(Constants.CogAvaloniaResourceFilePath);

        public static Image GetPowerIcon() {
            var power = GetImage(Constants.PowerAvaloniaResourceFilePath);
            RenderOptions.SetBitmapInterpolationMode(power, BitmapInterpolationMode.MediumQuality);
            return power;
        }

        private static Image GetImage(string avaloniaResourceFilePath) {
            var uri = new Uri(avaloniaResourceFilePath);
            var assetStream = AssetLoader.Open(uri);
            var bitmap = new Bitmap(assetStream);
            var image = new Image {
                Source = bitmap,
                Stretch = Stretch.UniformToFill,
            };
            return image;
        }
    }
}
