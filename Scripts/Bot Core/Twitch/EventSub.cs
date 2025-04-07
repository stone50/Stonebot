namespace Stonebot.Scripts.Bot_Core.Twitch {
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        // collector access token
        // only up to 1 of status, type, and userId should be specified
        public static async Task<HttpResponseMessage?> GetEventSubs(HttpClient client, string? status = null, string? type = null, string? userId = null, string? after = null) {
            Logger.Info("Getting event subs from Twitch.");

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
                Logger.Warning($"Could not get event subs from Twitch because client get attempt failed: {e}. Query params: {queryParams}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> DeleteEventSub(HttpClient client, string id) {
            Logger.Info($"Deleting event sub from Twitch. Id: {id}.");

            try {
                return await client.DeleteAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?id={id}");
            } catch (Exception e) {
                Logger.Warning($"Could not delete event sub from Twitch because client delete attempt failed: {e}. Id: {id}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> AddEventSub<T>(HttpClient client, T content) {
            Logger.Info("Adding event sub to Twitch.");

            try {
                return await client.PostAsJsonAsync("https://api.twitch.tv/helix/eventsub/subscriptions", content);
            } catch (Exception e) {
                Logger.Warning($"Could not add event sub to Twitch because client post as json attempt failed: {e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> SubscribeToChannelChatMessage(HttpClient client, string broadcasterUserId, string userId, string sessionId) {
            Logger.Info("Subscribing to channel chat message event sub on Twitch.");

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
                Logger.Warning("Could not subscribe to channel chat message event sub on Twitch because add event sub attempt failed.");
                return null;
            }

            return response;
        }
    }
}
