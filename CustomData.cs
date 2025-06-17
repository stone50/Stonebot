namespace Stonebot {
    using System.Collections.Concurrent;
    using System.Text.Json;

    internal static class CustomData {
        public static void Init() {
            Logger.Debug("Custom Data:");
            if (!File.Exists(Constants.CustomDataFilePath)) {
                return;
            }

            var customDataFileContents = File.ReadAllText(Constants.CustomDataFilePath);
            data = JsonSerializer.Deserialize(customDataFileContents, JsonContext.Default.ConcurrentDictionaryStringObject)!;
            foreach (var item in data) {
                Logger.Debug(item.Key, item.Value);
            }
        }

        public static object Get(string key) => data[key];

        public static void Set(string key, object value) => data[key] = value;

        public static void Save() {
            var contents = JsonSerializer.Serialize(data, JsonContext.Default.ConcurrentDictionaryStringObject);
            File.WriteAllText(Constants.CustomDataFilePath, contents);
        }

        private static ConcurrentDictionary<string, object> data = new();
    }
}
