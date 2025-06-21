namespace Stonebot {
    using System.Collections.Concurrent;
    using System.Text.Json;

    internal static class CustomData {
        public static void Init() {
            if (!File.Exists(Constants.CustomDataFilePath)) {
                return;
            }

            var customDataFileContents = File.ReadAllText(Constants.CustomDataFilePath);
            data = JsonSerializer.Deserialize(customDataFileContents, JsonContext.Default.ConcurrentDictionaryStringObject)!;
        }

        public static object? Get(string key) => data.GetValueOrDefault(key);

        public static void Set(string key, object value) => data[key] = value;

        public static bool Delete(string key) => data.Remove(key, out var _);

        public static bool Contains(string key) => data.ContainsKey(key);

        public static void Save() {
            var contents = JsonSerializer.Serialize(data, JsonContext.Default.ConcurrentDictionaryStringObject);
            File.WriteAllText(Constants.CustomDataFilePath, contents);
        }

        private static ConcurrentDictionary<string, object> data = new();
    }
}
