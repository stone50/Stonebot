namespace StoneBot.Scripts.Bot_Core {
    using Godot;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static class TwitchAPI {
        public static Process? Authorize(string clientId, string redirectUri, string[] scope, bool forceVerify = false, string? state = null) {
            var scopeParam = string.Join(" ", scope);

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&force_verify={forceVerify}&redirect_uri={redirectUri}&response_type=code&scope={scopeParam}";
            if (state is not null) {
                process.StartInfo.FileName += $"&state={state}";
            }

            try {
                _ = process.Start();
            } catch (Exception e) {
                GD.PushWarning($"Canot authorize because process.Start failed: {e}.");
                return null;
            }

            return process;
        }

        // no access token required
        public static async Task<HttpResponseMessage?> GetAccessToken(HttpClient client, string clientId, string clientSecret, string authorizationCode, string redirectUri) {
            try {
                return await client.PostAsync($"https://id.twitch.tv/oauth2/token?&client_id={clientId}&client_secret={clientSecret}&code={authorizationCode}&grant_type=authorization_code&redirect_uri={redirectUri}", null);
            } catch (Exception e) {
                GD.PushWarning($"Cannot get access token because client.PostAsync failed: {e}.");
                return null;
            }
        }

        // no access token required
        public static async Task<HttpResponseMessage?> RefreshAccessToken(HttpClient client, string clientId, string clientSecret, string refreshToken) {
            try {
                return await client.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=refresh_token&refresh_token={refreshToken}", null);
            } catch (Exception e) {
                GD.PushWarning($"Cannot refresh access token because client.PostAsync failed: {e}.");
                return null;
            }
        }

        // any access token required
        public static async Task<HttpResponseMessage?> GetUsers(HttpClient client, string[]? ids = null, string[]? logins = null) {
            var idParams = ids is null ? "" : string.Join("&", ids.Select(id => $"id={id}"));
            var loginParams = logins is null ? "" : string.Join("&", logins.Select(logins => $"login={logins}"));
            var queryParams = $"{idParams}{(ids is not null && logins is not null ? "&" : "")}{loginParams}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/users?{queryParams}");
            } catch (Exception e) {
                GD.PushWarning($"Cannot get users because client.GetAsync failed: {e}.");
                return null;
            }
        }

        // any access token required
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

        // any access token required
        public static async Task<HttpResponseMessage?> DeleteEventSub(HttpClient client, string id) {
            try {
                return await client.DeleteAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?id={id}");
            } catch (Exception e) {
                GD.PushWarning($"Cannot delete event sub because client.DeleteAsync failed: {e}.");
                return null;
            }
        }

        // any access token required
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

        // bot access token required
        public static async Task<HttpResponseMessage?> SendChatMessage(HttpClient client, string broadcasterId, string senderId, string message, string? replyParentMessageId = null) {
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
                GD.PushWarning($"Cannot send chat message because client.PostAsJsonAsync failed: {e}.");
                return null;
            }
        }
    }
}
