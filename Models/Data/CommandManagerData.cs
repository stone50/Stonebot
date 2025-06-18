namespace Stonebot.Models.Data {
    using System.Text.Json.Serialization;

    internal struct CommandManagerData {
        [JsonPropertyName("commands")]
        public CommandManagerDataCommand[] Commands { get; set; }
    }

    internal struct CommandManagerDataCommand {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("aliases")]
        public string[] Aliases { get; set; }
        [JsonPropertyName("cooldown_millis")]
        public int CooldownMillis { get; set; }
    }
}
