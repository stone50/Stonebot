namespace StoneBot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class CustomData {
        public List<string> Quotes;
        public int FeedCount;

        public static async Task<CustomData?> Create() {
            if (!File.Exists("data.json")) {
                return new(new() {
                    Quotes = Array.Empty<string>()
                });
            }

            string dataText;
            try {
                dataText = await File.ReadAllTextAsync("data.json");
            } catch {
                return null;
            }

            CustomDataData dataData;
            try {
                dataData = JsonSerializer.Deserialize<CustomDataData>(dataText);
            } catch {
                return null;
            }

            return new(dataData);
        }

        public CustomDataData ToDataData() => new() {
            Quotes = Quotes.ToArray(),
            FeedCount = FeedCount
        };

        private CustomData(CustomDataData dataData) {
            Quotes = new(dataData.Quotes);
            FeedCount = dataData.FeedCount;
        }
    }
}
