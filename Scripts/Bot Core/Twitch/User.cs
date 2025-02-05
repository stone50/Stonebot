namespace Stonebot.Scripts.Bot_Core.Twitch {
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        // collector access token
        public static async Task<HttpResponseMessage?> GetUsers(HttpClient client, string[]? ids = null, string[]? logins = null) {
            Logger.Info("Getting users from Twitch.");
            var idParams = ids is null ? "" : string.Join("&", ids.Select(id => $"id={id}"));
            var loginParams = logins is null ? "" : string.Join("&", logins.Select(logins => $"login={logins}"));
            var queryParams = $"{idParams}{(ids is not null && logins is not null ? "&" : "")}{loginParams}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/users?{queryParams}");
            } catch (Exception e) {
                Logger.Warning($"Cannot get users because client.GetAsync failed: {e}.");
                return null;
            }
        }

        // collector access token
        // first must be between 1 and 100 inclusive
        public static async Task<HttpResponseMessage?> GetModerators(HttpClient client, string broadcasterId, string[]? userId = null, string? first = null, string? after = null) {
            Logger.Info("Getting moderators from Twitch.");
            var queryParams = $"broadcaster_id={broadcasterId}";
            if (userId is not null) {
                if (userId.Length > 100) {
                    Logger.Warning("Cannot get moderators because userId.Length is greater than 100.");
                    return null;
                }

                queryParams += $"&{string.Join("&", userId.Select(id => $"user_id={id}"))}";
            }

            queryParams += first is null ? "" : $"&first={first}";
            queryParams += after is null ? "" : $"&after={after}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/moderation/moderators?{queryParams}");
            } catch (Exception e) {
                Logger.Warning($"Cannot get moderators because client.GetAsync failed: {e}.");
                return null;
            }
        }

        // collector access token
        // first must be between 1 and 100 inclusive
        public static async Task<HttpResponseMessage?> GetVIPs(HttpClient client, string broadcasterId, string[]? userId = null, string? first = null, string? after = null) {
            Logger.Info("Getting VIPs from Twitch.");
            var queryParams = $"broadcaster_id={broadcasterId}";
            if (userId is not null) {
                if (userId.Length > 100) {
                    Logger.Warning("Cannot get vips because userId.Length is greater than 100.");
                    return null;
                }

                queryParams += $"&{string.Join("&", userId.Select(id => $"user_id={id}"))}";
            }

            queryParams += first is null ? "" : $"&first={first}";
            queryParams += after is null ? "" : $"&after={after}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/channels/vips?{queryParams}");
            } catch (Exception e) {
                Logger.Warning($"Cannot get vips because client.GetAsync failed: {e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> GetBroadcasterSubscriptions(HttpClient client, string broadcasterId, string[]? userId = null, string? first = null, string? after = null, string? before = null) {
            Logger.Info("Getting subscriptions through Twitch.");
            var queryParams = $"broadcaster_id={broadcasterId}";
            if (userId is not null) {
                if (userId.Length > 100) {
                    Logger.Warning("Cannot get subscriptions because userId.Length is greater than 100.");
                    return null;
                }

                queryParams += $"&{string.Join("&", userId.Select(id => $"user_id={id}"))}";
            }

            queryParams += first is null ? "" : $"&first={first}";
            queryParams += after is null ? "" : $"&after={after}";
            queryParams += before is null ? "" : $"&before={before}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/subscriptions?{queryParams}");
            } catch (Exception e) {
                Logger.Warning($"Cannot get subscriptions because client.GetAsync failed: {e}.");
                return null;
            }
        }
    }
}
