namespace Stonebot.Models {
    using System.Text.Json.Serialization;

    internal struct PaginationData {
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }
    }
}
