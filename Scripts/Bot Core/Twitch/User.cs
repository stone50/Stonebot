namespace Stonebot.Scripts.Bot_Core.Twitch {
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using HttpClient = System.Net.Http.HttpClient;

    internal static partial class TwitchAPI {
        // collector access token
        public static async Task<HttpResponseMessage?> GetUsers(HttpClient client, string[]? ids = null, string[]? logins = null) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(GetUsers)}";
            Logger.Info($"{logPrefix}\n{nameof(ids)}:{JsonSerializer.Serialize(ids)}\n{nameof(logins)}:{JsonSerializer.Serialize(logins)}");

            var idParams = ids is null ? "" : string.Join("&", ids.Select(id => $"id={id}"));
            var loginParams = logins is null ? "" : string.Join("&", logins.Select(logins => $"login={logins}"));
            var queryParams = $"{idParams}{(ids is not null && logins is not null ? "&" : "")}{loginParams}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/users?{queryParams}");
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(client.GetAsync)} threw:{e}.");
                return null;
            }
        }

        // collector access token
        // first must be between 1 and 100 inclusive
        public static async Task<HttpResponseMessage?> GetModerators(HttpClient client, string broadcasterId, string[]? userId = null, string? first = null, string? after = null) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(GetModerators)}";
            Logger.Info($"{logPrefix}\n{nameof(broadcasterId)}:{broadcasterId}\n{nameof(userId)}:{JsonSerializer.Serialize(userId)}\n{nameof(first)}:{first}\n{nameof(after)}:{after}");

            var queryParams = $"broadcaster_id={broadcasterId}";
            if (userId is not null) {
                if (userId.Length > 100) {
                    Logger.Warning($"{logPrefix} | {nameof(userId.Length)} is > 100.");
                    return null;
                }

                queryParams += $"&{string.Join("&", userId.Select(id => $"user_id={id}"))}";
            }

            queryParams += first is null ? "" : $"&first={first}";
            queryParams += after is null ? "" : $"&after={after}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/moderation/moderators?{queryParams}");
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(client.GetAsync)} threw:{e}.");
                return null;
            }
        }

        // collector access token
        // first must be between 1 and 100 inclusive
        public static async Task<HttpResponseMessage?> GetVIPs(HttpClient client, string broadcasterId, string[]? userId = null, string? first = null, string? after = null) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(GetVIPs)}";
            Logger.Info($"{logPrefix}\n{nameof(broadcasterId)}:{broadcasterId}\n{nameof(userId)}:{JsonSerializer.Serialize(userId)}\n{nameof(first)}:{first}\n{nameof(after)}:{after}");

            var queryParams = $"broadcaster_id={broadcasterId}";
            if (userId is not null) {
                if (userId.Length > 100) {
                    Logger.Warning($"{logPrefix} | {nameof(userId.Length)} is > 100.");
                    return null;
                }

                queryParams += $"&{string.Join("&", userId.Select(id => $"user_id={id}"))}";
            }

            queryParams += first is null ? "" : $"&first={first}";
            queryParams += after is null ? "" : $"&after={after}";
            try {
                return await client.GetAsync($"https://api.twitch.tv/helix/channels/vips?{queryParams}");
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(client.GetAsync)} threw:{e}.");
                return null;
            }
        }

        // collector access token
        public static async Task<HttpResponseMessage?> GetBroadcasterSubscriptions(HttpClient client, string broadcasterId, string[]? userId = null, string? first = null, string? after = null, string? before = null) {
            var logPrefix = $"{nameof(TwitchAPI)} | {nameof(GetBroadcasterSubscriptions)}";
            Logger.Info($"{logPrefix}\n{nameof(broadcasterId)}:{broadcasterId}\n{nameof(userId)}:{JsonSerializer.Serialize(userId)}\n{nameof(first)}:{first}\n{nameof(after)}:{after}\n{nameof(before)}:{before}");

            var queryParams = $"broadcaster_id={broadcasterId}";
            if (userId is not null) {
                if (userId.Length > 100) {
                    Logger.Warning($"{logPrefix} | {nameof(userId.Length)} is > 100.");
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
                Logger.Warning($"{logPrefix} | {nameof(client.GetAsync)} threw:{e}.");
                return null;
            }
        }
    }
}
