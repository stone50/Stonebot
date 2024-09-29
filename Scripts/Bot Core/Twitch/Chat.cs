namespace Stonebot.Scripts.Bot_Core.Twitch {
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        // chatter access token
        public static async Task<HttpResponseMessage?> SendChatMessage(HttpClient client, string broadcasterId, string senderId, string message, string? replyParentMessageId = null) {
            Logger.Info("Sending chat message to Twitch.");
            dynamic content = new {
                broadcaster_id = broadcasterId,
                sender_id = senderId,
                message,
            };

            if (replyParentMessageId is not null) {
                content.reply_parent_message_id = replyParentMessageId;
            }

            try {
                return await client.PostAsJsonAsync("https://api.twitch.tv/helix/chat/messages", (object)content);
            } catch (Exception e) {
                Logger.Warning($"Cannot send chat message because client.PostAsJsonAsync failed: {e}.");
                return null;
            }
        }
    }
}
