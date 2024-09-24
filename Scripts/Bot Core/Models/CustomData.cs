namespace StoneBot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct CustomDataData {
        [JsonPropertyName("quotes")]
        public string[] Quotes { get; set; }
        [JsonPropertyName("feedCount")]
        public int FeedCount { get; set; }
    }
}
