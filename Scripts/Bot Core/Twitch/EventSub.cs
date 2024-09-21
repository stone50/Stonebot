namespace StoneBot.Scripts.Bot_Core.Twitch {
    using Godot;
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        // collector access token
        // only up to 1 of status, type, and userId should be specified
        public static async Task<HttpResponseMessage?> GetEventSubs(HttpClient client, string? status = null, string? type = null, string? userId = null, string? after = null) {
            var queryParams = "";
            if (status is not null) {
                queryParams = $"&status={status}";
            } else if (type is not null) {
                queryParams = $"&type={type}";
            } else if (userId is not null) {
                queryParams = $"&user_id={userId}";
            }

            if (after is not null) {
                if (queryParams != "") {
                    queryParams += "&";
                }

                queryParams += $"after={after}";
            }

            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?{queryParams}");
            } catch (Exception e) {
                GD.PushWarning($"Cannot get event subs because client.GetAsync failed: {e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> DeleteEventSub(HttpClient client, string id) {
            try {
                return await client.DeleteAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?id={id}");
            } catch (Exception e) {
                GD.PushWarning($"Cannot delete event sub because client.DeleteAsync failed: {e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> SubscribeToChannelChatMessage(HttpClient client, string broadcasterUserId, string userId, string sessionId) {
            var content = new {
                type = "channel.chat.message",
                version = "1",
                condition = new {
                    broadcaster_user_id = broadcasterUserId,
                    user_id = userId
                },
                transport = new {
                    method = "websocket",
                    session_id = sessionId
                }
            };

            try {
                return await client.PostAsJsonAsync("https://api.twitch.tv/helix/eventsub/subscriptions", content);
            } catch (Exception e) {
                GD.PushWarning($"Cannot subscribe to channel chat message because client.PostAsJsonAsync failed: {e}.");
                return null;
            }
        }
    }
}
