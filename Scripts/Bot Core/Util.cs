namespace Stonebot.Scripts.Bot_Core {
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class Util {
        public static async Task<T?> GetMessageAs<T>(HttpResponseMessage? message) where T : struct {
            var logPrefix = $"{nameof(Util)} | {nameof(GetMessageAs)}";
            Logger.Info(logPrefix);

            var stream = await GetStream(message);
            if (stream is null) {
                Logger.Warning($"{logPrefix} | {nameof(GetStream)} result is null.");
                return null;
            }

            T messageAsT;
            try {
                messageAsT = await JsonSerializer.DeserializeAsync<T>(stream);
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(JsonSerializer.DeserializeAsync)} threw: {e}");
                return null;
            }

            return messageAsT;
        }
        public static async Task<T?> GetMessageAs<T>(Task<HttpResponseMessage?> messageTask) where T : struct => await GetMessageAs<T>(await messageTask);

        public static async Task<Stream?> GetStream(HttpResponseMessage? message) {
            var logPrefix = $"{nameof(Util)} | {nameof(GetStream)}";
            Logger.Info(logPrefix);

            if (message is null) {
                Logger.Warning($"{logPrefix} | {nameof(message)} is null.");
                return null;
            }

            if (!message.IsSuccessStatusCode) {
                Logger.Warning($"{logPrefix} | {nameof(message.IsSuccessStatusCode)} is false.");
                return null;
            }

            try {
                return await message.Content.ReadAsStreamAsync();
            } catch (Exception e) {
                Logger.Warning($"{logPrefix} | {nameof(message.Content.ReadAsStreamAsync)} threw: {e}.");
                return null;
            }
        }
        public static async Task<Stream?> GetStream(Task<HttpResponseMessage?> messageTask) => await GetStream(await messageTask);

        public static bool GetIsSuccess(HttpResponseMessage? message) => message is not null && message.IsSuccessStatusCode;
        public static async Task<bool> GetIsSuccess(Task<HttpResponseMessage?> messageTask) => GetIsSuccess(await messageTask);
    }
}
