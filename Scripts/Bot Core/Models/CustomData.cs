﻿namespace Stonebot.Scripts.Bot_Core.Models {
    using System.Text.Json.Serialization;

    internal struct CustomDataData {
        [JsonPropertyName("quotes")]
        public string[] Quotes { get; set; }
        [JsonPropertyName("feedCount")]
        public int FeedCount { get; set; }
        [JsonPropertyName("feedRecord")]
        public int FeedRecord { get; set; }
        [JsonPropertyName("feedRecordHolder")]
        public string FeedRecordHolder { get; set; }
        [JsonPropertyName("discordInvite")]
        public string DiscordInvite { get; set; }
        [JsonPropertyName("youTubeLink")]
        public string YouTubeLink { get; set; }
    }
}
