namespace Stonebot.Scripts.Bot_Core.Twitch {
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        // chatter access token
        public static async Task<HttpResponseMessage?> SendChatMessage(HttpClient client, string broadcasterId, string senderId, string message, string? replyParentMessageId = null) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(SendChatMessage)}";
            Logger.Info($"{logPrefix}\n{nameof(broadcasterId)}: {broadcasterId}\n{nameof(senderId)}: {senderId}\n{nameof(message)}: {message}\n{nameof(replyParentMessageId)}: {replyParentMessageId}");

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
                Logger.Warning($"{logPrefix} | PostAsJsonAsync threw: {e}.");
                return null;
            }
        }
    }
}
