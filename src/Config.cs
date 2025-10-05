namespace Stonebot {
    internal static class Config {
        public static string BroadcasterUsername = "";
        public static string ClientId = "";
        public static bool ShouldFilterInvisibleText = true;
        public static int NumMaxLogFiles = Constants.NumMaxLogFilesDefault;

        public static void Init() {
            if (File.Exists(Constants.ConfigFilePath)) {
                Load();
            }
        }

        public static void Save() {
            using var writer = new BinaryWriter(File.Open(Constants.ConfigFilePath, FileMode.Create));
            writer.Write(BroadcasterUsername);
            writer.Write(ClientId);
            writer.Write(ShouldFilterInvisibleText);
            writer.Write(NumMaxLogFiles);
        }

        private static void Load() {
            using var reader = new BinaryReader(File.Open(Constants.ConfigFilePath, FileMode.Open));
            BroadcasterUsername = reader.ReadString();
            ClientId = reader.ReadString();
            ShouldFilterInvisibleText = reader.ReadBoolean();
            NumMaxLogFiles = reader.ReadInt32();
        }
    }
}
