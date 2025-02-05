namespace Stonebot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct UsersPermissionData {
        [JsonPropertyName("data")]
        public SimpleUserData[] Data { get; set; }
        [JsonPropertyName("pagination")]
        public PaginationData Pagination { get; set; }
    }
}
