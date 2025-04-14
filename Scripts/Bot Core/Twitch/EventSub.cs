namespace Stonebot.Scripts.Bot_Core.Twitch {
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        // collector access token
        // only up to 1 of status, type, and userId should be specified
        public static async Task<HttpResponseMessage?> GetEventSubs(HttpClient client, string? status = null, string? type = null, string? userId = null, string? after = null) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(GetEventSubs)}";
            Logger.Info($"{logPrefix}/n{nameof(status)}: {status}/n{nameof(type)}: {type}/n{nameof(userId)}: {userId}/n{nameof(after)}: {after}");

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
                Logger.Warning($"{logPrefix} | {nameof(client.GetAsync)} threw: {e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> DeleteEventSub(HttpClient client, string id) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(DeleteEventSub)}";
            Logger.Info($"{logPrefix}/n{nameof(id)}: {id}");

            try {
                return await client.DeleteAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?id={id}");
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(client.DeleteAsync)} threw: {e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> AddEventSub<T>(HttpClient client, T content) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(AddEventSub)}";
            Logger.Info($"{logPrefix}/n{nameof(content)}: {JsonSerializer.Serialize(content)}");

            try {
                return await client.PostAsJsonAsync("https://api.twitch.tv/helix/eventsub/subscriptions", content);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | PostAsJsonAsync threw: {e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> SubscribeToChannelChatMessage(HttpClient client, string broadcasterUserId, string userId, string sessionId) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(SubscribeToChannelChatMessage)}";
            Logger.Info($"{logPrefix}/n{nameof(broadcasterUserId)}: {broadcasterUserId}/n{nameof(userId)}: {userId}");

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
            var response = await AddEventSub(client, content);
            if (response is null) {
                Logger.Warning($"{logPrefix} | {nameof(AddEventSub)} result is null.");
                return null;
            }

            return response;
        }
    }
}
