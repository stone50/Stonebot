namespace StonebotCore.Models {
    using System;

    internal class FeedData {
        public int Count { get; set; }
        public DateTime DateLastUsed { get; set; }
        public int RecordCount { get; set; }
        public string RecordHolder { get; set; } = "";
        public DateTime DateRecordSet { get; set; }
    }
}
