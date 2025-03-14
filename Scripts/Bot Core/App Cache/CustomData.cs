namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class CustomData {
        public List<string> Quotes;
        public int FeedCount;
        public int FeedRecord;
        public string FeedRecordHolder;
        public string DiscordInvite;
        public string YouTubeLink;

        public static async Task<CustomData?> Create() {
            Logger.Info("Creating custom data.");
            if (!File.Exists(Constants.DataFilePath)) {
                return new(new() {
                    Quotes = []
                });
            }

            string dataText;
            try {
                dataText = await File.ReadAllTextAsync(Constants.DataFilePath);
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
            Quotes = [.. Quotes],
            FeedCount = FeedCount,
            FeedRecord = FeedRecord,
            FeedRecordHolder = FeedRecordHolder,
            DiscordInvite = DiscordInvite,
            YouTubeLink = YouTubeLink
        };

        private CustomData(CustomDataData dataData) {
            Quotes = [.. dataData.Quotes];
            FeedCount = dataData.FeedCount;
            FeedRecord = dataData.FeedRecord;
            FeedRecordHolder = dataData.FeedRecordHolder;
            DiscordInvite = dataData.DiscordInvite;
            YouTubeLink = dataData.YouTubeLink;
        }
    }
}
