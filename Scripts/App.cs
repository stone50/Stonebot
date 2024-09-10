namespace StoneBot.Scripts {
    using Godot;
    using StoneBot.Scripts.Middleware;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;
    using RandomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator;

    internal partial class App : Node {
        private TcpServer? TwitchEventSubServer;
        private HttpClient? TwitchHttpClient;
        private string? BroadcasterId;

        public App() {
            _ = Configuration.Init();
            _ = Task.Run(Init);
        }

        public async Task<bool> Init() {
            if (Configuration.TwitchStoneBotClientId is null) {
                GD.PushWarning("Cannot init app because TwitchStoneBotClientId is null.");
                return false;
            }

            var secret = GetSecret(32);
            if (secret is null) {
                GD.PushWarning("Cannot init app because GetSecret failed.");
                return false;
            }

            TwitchEventSubServer = GetTwitchEventSubServer(secret);
            if (TwitchEventSubServer is null) {
                GD.PushWarning("Cannot init app because TwitchEventSubServer is null.");
                return false;
            }

            TwitchHttpClient = await GetTwitchHttpClient();
            if (TwitchHttpClient is null) {
                GD.PushWarning("Cannot init app because GetTwitchEventSubClient failed.");
                return false;
            }

            var potentialBroadcasterData = await Broadcaster.GetBroadcasterData(TwitchHttpClient);
            if (potentialBroadcasterData is null) {
                GD.PushWarning("Cannot init app because GetBroadcasterData failed.");
                return false;
            }

            var broadcasterData = (Broadcaster.UserData)potentialBroadcasterData;
            BroadcasterId = broadcasterData.Id;

            var isChatEventSubConnected = await ChannelChatMessage.Connect(BroadcasterId, secret);
            if (!isChatEventSubConnected) {
                GD.PushWarning("Cannot init app because chat event sub connection failed.");
                return false;
            }

            return true;
        }

        private static string? GetSecret(int numBytes) {
            byte[] bytes;
            try {
                bytes = RandomNumberGenerator.GetBytes(numBytes);
            } catch (Exception e) {
                GD.PushWarning($"Could not get bytes: {e}.");
                return null;
            }

            string secret;
            try {
                secret = Convert.ToBase64String(bytes);
            } catch (Exception e) {
                GD.PushWarning($"Could not get bytes: {e}.");
                return null;
            }

            return secret;
        }

        private static async Task<HttpClient?> GetTwitchHttpClient() {
            if (Configuration.TwitchStoneBotClientId is null) {
                GD.PushWarning("Cannot get Twitch event sub client because TwitchStoneBotClientId is null.");
                return null;
            }

            var potentialAccessTokenData = await AccessToken.GetUserAccessTokenData();
            if (potentialAccessTokenData is null) {
                GD.PushWarning("Cannot get Twitch event sub client because GetAccessTokenData failed.");
                return null;
            }

            var accessTokenData = (AccessToken.UserAccessTokenData)potentialAccessTokenData;

            var client = new HttpClient();

            try {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessTokenData.AccessToken}");
            } catch (Exception e) {
                GD.PushWarning($"Could not add Authorization header to Twitch event sub client: {e}.");
                return null;
            }

            try {
                client.DefaultRequestHeaders.Add("Client-Id", Configuration.TwitchStoneBotClientId);
            } catch (Exception e) {
                GD.PushWarning($"Could not add Client-Id header to Twitch event sub client: {e}.");
                return null;
            }

            return client;
        }

        private static TcpServer? GetTwitchEventSubServer(string secret) {
            IPAddress localhost;
            try {
                localhost = IPAddress.Parse("127.0.0.1");
            } catch (Exception e) {
                GD.PushWarning($"Could not parse IP address: {e}.");
                return null;
            }

            TcpServer server;
            try {
                server = new TcpServer(localhost, 443);
            } catch (Exception e) {
                GD.PushWarning($"Could not create server: {e}.");
                return null;
            }

            bool didTwitchEventSubServerStart;
            try {
                didTwitchEventSubServerStart = server.Start();
            } catch (Exception e) {
                GD.PushWarning($"Could not start server: {e}.");
                return null;
            }

            if (!didTwitchEventSubServerStart) {
                GD.PushWarning($"Cannot get twitch event sub server because server start failed.");
                return null;
            }

            server.RequestHandler += (sender, args) => {
                // TODO: process request
                GD.Print(DateTime.Now);
                GD.Print(args.Request.Method);
                GD.Print(args.Request.URI);
            };

            return server;
        }
    }
}
