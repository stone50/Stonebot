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
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("permission_level")]
        public UserPermission.Level PermissionLevel { get; set; }
        [JsonPropertyName("cooldown_seconds")]
        public int CooldownSeconds { get; set; }
    }
}
