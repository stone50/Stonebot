namespace Stonebot.Scripts.Bot_Core.App_Cache {
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class CustomData {
        public readonly List<string> Quotes;
        public int FeedCount;
        public int FeedRecord;
        public string FeedRecordHolder;
        public string DiscordInvite;
        public string YouTubeLink;

        public CustomDataData DataData => new() {
            Quotes = [.. Quotes],
            FeedCount = FeedCount,
            FeedRecord = FeedRecord,
            FeedRecordHolder = FeedRecordHolder,
            DiscordInvite = DiscordInvite,
            YouTubeLink = YouTubeLink
        };

        public string MaskedSerialized => JsonSerializer.Serialize(new {
            Quotes,
            FeedCount,
            FeedRecord,
            FeedRecordHolder,
            DiscordInvite,
            YouTubeLink
        });

        public static async Task<CustomData?> Create() {
            var logPrefix = $"{nameof(CustomData)} | {nameof(Create)}";
            Logger.Info(logPrefix);

            if (!File.Exists(Constants.DataFilePath)) {
                return new(new() { Quotes = [] });
            }

            string dataText;
            try {
                dataText = await File.ReadAllTextAsync(Constants.DataFilePath);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(File.ReadAllTextAsync)} threw: {e}.\n{nameof(Constants.DataFilePath)}: {Scripts.Util.GetMaskedPath(Constants.DataFilePath)}");
                return null;
            }

            CustomDataData dataData;
            try {
                dataData = JsonSerializer.Deserialize<CustomDataData>(dataText);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(JsonSerializer.Deserialize)} threw: {e}.");
                return null;
            }

            return new(dataData);
        }

        private CustomData(CustomDataData dataData) {
            Logger.Info($"{nameof(CustomData)} | Constructor\n{nameof(dataData)}: {dataData.MaskedSerialized}");

            Quotes = [.. dataData.Quotes];
            FeedCount = dataData.FeedCount;
            FeedRecord = dataData.FeedRecord;
            FeedRecordHolder = dataData.FeedRecordHolder;
            DiscordInvite = dataData.DiscordInvite;
            YouTubeLink = dataData.YouTubeLink;
        }
    }
}
