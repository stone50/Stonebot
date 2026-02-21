namespace StonebotDaemon.Models {
    using System;

    internal class Quote {
        public string QuoteText { get; set; } = "";
        public string SpokenBy { get; set; } = "";
        public DateTime? DateAdded { get; set; }
        public string? AddedBy { get; set; }
    }
}
