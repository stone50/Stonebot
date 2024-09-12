namespace StoneBot.Scripts {
    using Godot;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static class TwitchAPI {
        public static Process? Authorize(string clientId, string redirectURI, string[] scope, bool forceVerify = false, string? state = null) {
            string scopeParam;
            try {
                scopeParam = string.Join(" ", scope);
            } catch (Exception e) {
                GD.PushWarning($"Could not format scope: {e}.");
                return null;
            }

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&force_verify={forceVerify}&redirect_uri={redirectURI}&response_type=code&scope={scopeParam}";
            if (state is not null) {
                process.StartInfo.FileName += $"&state={state}";
            }

            bool processStarted;
            try {
                processStarted = process.Start();
            } catch (Exception e) {
                GD.PushWarning($"Could not start authorization process: {e}.");
                return null;
            }

            if (!processStarted) {
                GD.PushWarning($"Cannot authorize because process start failed.");
                return null;
            }

            return process;
        }

        public static async Task<HttpResponseMessage?> GetUserAccessToken(HttpClient client, string clientId, string clientSecret, string authorizationCode, string redirectURI) {
            try {
                return await client.PostAsync($"https://id.twitch.tv/oauth2/token?&client_id={clientId}&client_secret={clientSecret}&code={authorizationCode}&grant_type=authorization_code&redirect_uri={redirectURI}", null);
            } catch (Exception e) {
                GD.PushWarning($"Could not get access token: {e}.");
                return null;
            }
        }

        public static async Task<HttpResponseMessage?> GetAppAccessToken(HttpClient client, string clientId, string clientSecret) {
            try {
                return await client.PostAsync($"https://id.twitch.tv/oauth2/token?&client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials", null);
            } catch (Exception e) {
                GD.PushWarning($"Could not get access token: {e}.");
                return null;
            }
        }

        public static async Task<HttpResponseMessage?> GetUsers(HttpClient client, string[]? ids = null, string[]? logins = null) {
            if (ids is null && logins is null) {
                GD.PushWarning("Cannot get users because ids and logins are both null.");
                return null;
            }

            string idParams;
            try {
                idParams = ids is null ? "" : string.Join("&", ids.Select(id => $"id={id}"));
            } catch (Exception e) {
                GD.PushWarning($"Could not format id params: {e}.");
                return null;
            }

            string loginParams;
            try {
                loginParams = logins is null ? "" : string.Join("&", logins.Select(logins => $"logins={logins}"));
            } catch (Exception e) {
                GD.PushWarning($"Could not format login params: {e}.");
                return null;
            }

            var queryParams = idParams + (ids is not null && logins is not null ? "&" : "") + loginParams;

            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/users?{queryParams}");
            } catch (Exception e) {
                GD.PushWarning($"Could not get users: {e}.");
                return null;
            }
        }

        // Only up to 1 of status, type, and userId should be specified
        public static async Task<HttpResponseMessage?> GetEventSubs(HttpClient client, string? status = null, string? type = null, string? userId = null, string? after = null) {
            var queryParam = "";
            if (status is not null) {
                queryParam = $"&status={status}";
            } else if (type is not null) {
                queryParam = $"&type={type}";
            } else if (userId is not null) {
                queryParam = $"&user_id={userId}";
            }

            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?{queryParam}");
            } catch (Exception e) {
                GD.PushWarning($"Could not get event subs: {e}.");
                return null;
            }
        }

        public static async Task<HttpResponseMessage?> DeleteEventSub(HttpClient client, string id) {
            try {
                return await client.DeleteAsync($"https://api.twitch.tv/helix/eventsub/subscriptions?id={id}");
            } catch (Exception e) {
                GD.PushWarning(e.Message);
                return null;
            }
        }

        public static async Task<HttpResponseMessage?> ChannelChatMessageWebhook(HttpClient client, string broadcasterUserId, string userId, string callback, string secret) {
            var content = new {
                type = "channel.chat.message",
                version = "1",
                condition = new {
                    broadcaster_user_id = broadcasterUserId,
                    user_id = userId,
                },
                transport = new {
                    method = "webhook",
                    callback,
                    secret,
                },
            };

            try {
                return await client.PostAsJsonAsync("https://api.twitch.tv/helix/eventsub/subscriptions", (object)content);
            } catch (Exception e) {
                GD.PushWarning($"Could not connect channel chat message webhook: {e}.");
                return null;
            }
        }

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
                GD.PushWarning($"Could not send chat message: {e}.");
                return null;
            }
        }
    }
}
