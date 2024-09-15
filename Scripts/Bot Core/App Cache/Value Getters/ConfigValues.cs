namespace StoneBot.Scripts.Bot_Core.App_Cache.Value_Getters {
    using Godot;
    using Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static partial class ValueGetters {
        public static async Task<ConfigValues?> GetConfigValues() {
            string jsonText;
            try {
                jsonText = await File.ReadAllTextAsync("config.json");
            } catch (Exception e) {
                GD.PushWarning($"Could not read text from config file: {e}.");
                return null;
            }

            ConfigValues values;
            try {
                values = JsonSerializer.Deserialize<ConfigValues>(jsonText);
            } catch (Exception e) {
                GD.PushWarning($"Could not deserialize config values: {e}.");
                return null;
            }

            return values;
        }
    }
}
