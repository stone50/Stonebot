namespace Stonebot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct PaginationData {
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }
}
