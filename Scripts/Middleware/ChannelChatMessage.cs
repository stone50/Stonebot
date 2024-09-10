namespace StoneBot.Scripts.Middleware {
    using Godot;
    using System;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static class ChannelChatMessage {
        public struct EventSubData {

        }

        public struct EventSubsData {
            public int Total { get; set; }
            public EventSubData[] Datas { get; set; }
            public int TotalCost { get; set; }
            public int MaxTotalCost { get; set; }
            public
        }

        public static async Task<bool> Get(string broadcasterId, string secret) {
            var client = await GetClient();
            if (client is null) {
                GD.PushWarning("Cannot connect chat event sub because GetClient failed.");
                return false;
            }

            var response = await TwitchAPI.GetEventSubs(client, null, "channel.chat.message");
            if (res) {

            }
        }

        public static async Task<bool> Clear(string broadcasterId, string secret) {
            var client = await GetClient();
            if (client is null) {
                GD.PushWarning("Cannot connect chat event sub because GetClient failed.");
                return false;
            }

            var response = await TwitchAPI.GetEventSubs(client);
        }

        public static async Task<bool> Connect(string broadcasterId, string secret) {
            var client = await GetClient();
            if (client is null) {
                GD.PushWarning("Cannot connect chat event sub because GetClient failed.");
                return false;
            }

            var responseString = Util.VerifyHttpResponseMessage(await TwitchAPI.ChannelChatMessageWebhook(client, broadcasterId, broadcasterId, "https://localhost:443", secret));
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
