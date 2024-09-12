namespace StoneBot.Scripts.Middleware {
    using Godot;
    using System;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static class ChannelChatMessage {
        public struct ConditionData {
            [JsonPropertyName("broadcaster_user_id")]
            public string BroadcasterUserId { get; set; }
            [JsonPropertyName("user_id")]
            public string UserId { get; set; }
        }

        public struct TransportData {
            [JsonPropertyName("method")]
            public string Method { get; set; }
            [JsonPropertyName("callback")]
            public string Callback { get; set; }
        }

        public struct EventSubData {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("status")]
            public string Status { get; set; }
            [JsonPropertyName("type")]
            public string Type { get; set; }
            [JsonPropertyName("version")]
            public string Version { get; set; }
            [JsonPropertyName("condition")]
            public ConditionData Condition { get; set; }
            [JsonPropertyName("created_at")]
            public string CreatedAt { get; set; }
            [JsonPropertyName("transport")]
            public TransportData Transport { get; set; }
        }

        public struct PaginationData {
            [JsonPropertyName("cursor")]
            public string? Cursor { get; set; }
        }

        public struct EventSubsData {
            [JsonPropertyName("data")]
            public EventSubData[] Data { get; set; }
            [JsonPropertyName("total")]
            public int Total { get; set; }
            [JsonPropertyName("total_cost")]
            public int TotalCost { get; set; }
            [JsonPropertyName("Max_total_cost")]
            public int MaxTotalCost { get; set; }
            [JsonPropertyName("pagination")]
            public PaginationData Pagination { get; set; }
        }

        public static async Task<EventSubsData?> Get(HttpClient? client = null) {
            var requestClient = client ?? await GetClient();
            if (requestClient is null) {
                GD.PushWarning("Cannot connect chat event sub because client is null.");
                return null;
            }

            var eventSubsData = await Util.ProcessHttpResponseMessage<EventSubsData>(await TwitchAPI.GetEventSubs(requestClient, null, "channel.chat.message"));
            if (eventSubsData is null) {
                GD.PushWarning("Cannot get event subs data because ProcessHttpResponseMessage failed.");
                return null;
            }

            return eventSubsData;
        }

        // returns the number of event subs that were not deleted
        public static async Task<int> Clear() {
            var client = await GetClient();
            if (client is null) {
                GD.PushWarning("Cannot connect chat event sub because GetClient failed.");
                return 0;
            }

            var potentialEventSubsData = await Get(client);
            if (potentialEventSubsData is null) {
                GD.PushWarning("Cannot clear even subs because Get failed.");
                return 0;
            }

            var eventSubsData = (EventSubsData)potentialEventSubsData;

            var numEventSubsDeleted = 0;
            foreach (var eventSubData in eventSubsData.Data) {
                var responseString = await Util.VerifyHttpResponseMessage(await TwitchAPI.DeleteEventSub(client, eventSubData.Id));
                if (responseString is null) {
                    GD.PushWarning($"Cannot delete event sub because VerifyHttpResponseMessage failed: {responseString}.");
                    continue;
                }

                numEventSubsDeleted++;
            }

            return eventSubsData.Data.Length - numEventSubsDeleted;
        }

        public static async Task<bool> Connect(string broadcasterId, string secret) {
            var client = await GetClient();
            if (client is null) {
                GD.PushWarning("Cannot connect chat event sub because GetClient failed.");
                return false;
            }

            var responseString = await Util.VerifyHttpResponseMessage(await TwitchAPI.ChannelChatMessageWebhook(client, broadcasterId, broadcasterId, "https://localhost:443", secret));
            if (responseString is null) {
                GD.PushWarning("Cannot connect chat event sub because VerifyHttpResponseMessage failed.");
                return false;
            }

            GD.Print(responseString);

            return true;
        }

        private static async Task<HttpClient?> GetClient() {
            if (Configuration.TwitchStoneBotClientId is null) {
                GD.PushWarning("Cannot connect chat event sub because TwitchStoneBotClientId is null.");
                return null;
            }

            var potentialAccessTokenData = await AccessToken.GetAppAccessTokenData();
            if (potentialAccessTokenData is null) {
                GD.PushWarning("Cannot connect chat event sub because GetAppAccessTokenData failed.");
                return null;
            }

            var accessTokenData = (AccessToken.AppAccessTokenData)potentialAccessTokenData;

            var client = new HttpClient();
            try {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenData.AccessToken}");
            } catch (Exception e) {
                GD.PushWarning($"Could not add Authorization header: {e}.");
                return null;
            }

            try {
                client.DefaultRequestHeaders.Add("Client-Id", Configuration.TwitchStoneBotClientId);
            } catch (Exception e) {
                GD.PushWarning($"Could not add Client-Id header: {e}.");
                return null;
            }

            return client;
        }
    }
}
