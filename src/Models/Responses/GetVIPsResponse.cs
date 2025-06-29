namespace Stonebot.Models.Responses {
    using System.Text.Json.Serialization;

    internal struct GetVIPsResponse {
        [JsonPropertyName("data")]
        public GetVIPsResponseDataPoint[] Data { get; set; }
        [JsonPropertyName("pagination")]
        public GetVIPsResponsePagination Pagination { get; set; }
    }
    internal struct GetVIPsResponseDataPoint {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("user_login")]
        public string UserLogin { get; set; }
    }
    internal struct GetVIPsResponsePagination {
        [JsonPropertyName("cursor")]
        public string Cursor { get; set; }
    }
}
