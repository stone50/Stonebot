namespace Stonebot.UI {
    using Avalonia.Media;

    internal static class MainTheme {
        public static readonly Color PrimaryColor1 = Color.Parse("#2a2a40");
        public static readonly Color PrimaryColor2 = Color.Parse("#242436");
        public static readonly Color PrimaryColor3 = Color.Parse("#1e1e2c");
        public static readonly Color PrimaryColor4 = Color.Parse("#191923");
        public static readonly Color PrimaryColor5 = Color.Parse("#13131a");
        public static readonly Color PrimaryColor6 = Color.Parse("#0b0a10");

        public static readonly Color AccentColor1 = Color.Parse("#4cb04f");
        public static readonly Color AccentColor2 = Color.Parse("#419043");
        public static readonly Color AccentColor3 = Color.Parse("#367236");
        public static readonly Color AccentColor4 = Color.Parse("#2b552a");
        public static readonly Color AccentColor5 = Color.Parse("#20391f");
        public static readonly Color AccentColor6 = Color.Parse("#152013");

        public static readonly Color DangerColor1 = Color.Parse("#b41c2b");
        public static readonly Color DangerColor2 = Color.Parse("#851d22");
        public static readonly Color DangerColor3 = Color.Parse("#581919");
        public static readonly Color DangerColor4 = Color.Parse("#2f1310");

        public static readonly Color SuccessColor1 = Color.Parse("#009f42");
        public static readonly Color SuccessColor2 = Color.Parse("#167533");
        public static readonly Color SuccessColor3 = Color.Parse("#184d25");
        public static readonly Color SuccessColor4 = Color.Parse("#132916");

        public static readonly Color WarningColor1 = Color.Parse("#f0ad4e");
        public static readonly Color WarningColor2 = Color.Parse("#af7f3c");
        public static readonly Color WarningColor3 = Color.Parse("#71532a");
        public static readonly Color WarningColor4 = Color.Parse("#392b19");

        public static readonly Color InfoColor1 = Color.Parse("#388cfa");
        public static readonly Color InfoColor2 = Color.Parse("#3267b5");
        public static readonly Color InfoColor3 = Color.Parse("#284475");
        public static readonly Color InfoColor4 = Color.Parse("#1a253b");

        public static readonly Color NeutralColor1 = Color.Parse("#ffffff");
        public static readonly Color NeutralColor2 = Color.Parse("#c6c6c6");
        public static readonly Color NeutralColor3 = Color.Parse("#919191");
        public static readonly Color NeutralColor4 = Color.Parse("#5e5e5e");
        public static readonly Color NeutralColor5 = Color.Parse("#303030");
        public static readonly Color NeutralColor6 = Color.Parse("#000000");

        public static readonly IImmutableBrush PrimaryBrush1 = GetImmutableBrush(PrimaryColor1);
        public static readonly IImmutableBrush PrimaryBrush2 = GetImmutableBrush(PrimaryColor2);
        public static readonly IImmutableBrush PrimaryBrush3 = GetImmutableBrush(PrimaryColor3);
        public static readonly IImmutableBrush PrimaryBrush4 = GetImmutableBrush(PrimaryColor4);
        public static readonly IImmutableBrush PrimaryBrush5 = GetImmutableBrush(PrimaryColor5);
        public static readonly IImmutableBrush PrimaryBrush6 = GetImmutableBrush(PrimaryColor6);

        public static readonly IImmutableBrush AccentBrush1 = GetImmutableBrush(AccentColor1);
        public static readonly IImmutableBrush AccentBrush2 = GetImmutableBrush(AccentColor2);
        public static readonly IImmutableBrush AccentBrush3 = GetImmutableBrush(AccentColor3);
        public static readonly IImmutableBrush AccentBrush4 = GetImmutableBrush(AccentColor4);
        public static readonly IImmutableBrush AccentBrush5 = GetImmutableBrush(AccentColor5);
        public static readonly IImmutableBrush AccentBrush6 = GetImmutableBrush(AccentColor6);

        public static readonly IImmutableBrush DangerBrush1 = GetImmutableBrush(DangerColor1);
        public static readonly IImmutableBrush DangerBrush2 = GetImmutableBrush(DangerColor2);
        public static readonly IImmutableBrush DangerBrush3 = GetImmutableBrush(DangerColor3);
        public static readonly IImmutableBrush DangerBrush4 = GetImmutableBrush(DangerColor4);

        public static readonly IImmutableBrush SuccessBrush1 = GetImmutableBrush(SuccessColor1);
        public static readonly IImmutableBrush SuccessBrush2 = GetImmutableBrush(SuccessColor2);
        public static readonly IImmutableBrush SuccessBrush3 = GetImmutableBrush(SuccessColor3);
        public static readonly IImmutableBrush SuccessBrush4 = GetImmutableBrush(SuccessColor4);

        public static readonly IImmutableBrush WarningBrush1 = GetImmutableBrush(WarningColor1);
        public static readonly IImmutableBrush WarningBrush2 = GetImmutableBrush(WarningColor2);
        public static readonly IImmutableBrush WarningBrush3 = GetImmutableBrush(WarningColor3);
        public static readonly IImmutableBrush WarningBrush4 = GetImmutableBrush(WarningColor4);

        public static readonly IImmutableBrush InfoBrush1 = GetImmutableBrush(InfoColor1);
        public static readonly IImmutableBrush InfoBrush2 = GetImmutableBrush(InfoColor2);
        public static readonly IImmutableBrush InfoBrush3 = GetImmutableBrush(InfoColor3);
        public static readonly IImmutableBrush InfoBrush4 = GetImmutableBrush(InfoColor4);

        public static readonly IImmutableBrush NeutralBrush1 = GetImmutableBrush(NeutralColor1);
        public static readonly IImmutableBrush NeutralBrush2 = GetImmutableBrush(NeutralColor2);
        public static readonly IImmutableBrush NeutralBrush3 = GetImmutableBrush(NeutralColor3);
        public static readonly IImmutableBrush NeutralBrush4 = GetImmutableBrush(NeutralColor4);
        public static readonly IImmutableBrush NeutralBrush5 = GetImmutableBrush(NeutralColor5);
        public static readonly IImmutableBrush NeutralBrush6 = GetImmutableBrush(NeutralColor6);

        public static readonly FontFamily Font = new("avares://Stonebot/Assets/JetBrainsMono-Regular.ttf#JetBrains Mono");

        private static IImmutableBrush GetImmutableBrush(Color color) => new SolidColorBrush(color).ToImmutable();
    }
}
