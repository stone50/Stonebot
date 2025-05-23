namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal struct GetModeratorsResponse {
        [JsonPropertyName("data")]
        public GetModeratorsResponseDataPoint[] Data { get; set; }
        [JsonPropertyName("pagination")]
        public GetModeratorsResponsePagination Pagination { get; set; }
    }
    internal struct GetModeratorsResponseDataPoint {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }
    }
    internal struct GetModeratorsResponsePagination {
        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }
}
