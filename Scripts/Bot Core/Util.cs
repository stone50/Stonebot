namespace Stonebot.Scripts.Bot_Core {
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class Util {
        public static async Task<T?> GetMessageAs<T>(HttpResponseMessage? message) where T : struct {
            Logger.Info($"Getting http response message as {typeof(T).FullName}.");

            var successfulString = await GetSuccessfulString(message);
            if (successfulString is null) {
                Logger.Warning($"Could not get http response message as {typeof(T).FullName} because get successful string attempt failed.");
                return null;
            }

            T messageAsT;
            try {
                messageAsT = JsonSerializer.Deserialize<T>(successfulString);
            } catch (Exception e) {
                Logger.Warning($"Could not get http message as {typeof(T).FullName} because json serializer deserialize attempt failed: {e}. Successful string: {successfulString}.");
                return null;
            }

            return messageAsT;
        }
        public static async Task<T?> GetMessageAs<T>(Task<HttpResponseMessage?> messageTask) where T : struct => await GetMessageAs<T>(await messageTask);

        public static async Task<string?> GetSuccessfulString(HttpResponseMessage? message) {
            Logger.Info("Getting successful string from http response message.");

            if (message is null) {
                Logger.Warning("Could not get successful string from http response message because message is null.");
                return null;
            }

            if (!message.IsSuccessStatusCode) {
                Logger.Warning($"Could not get successful string from http response message because message is success status code is false.");
                return null;
            }

            string successfulString;
            try {
                successfulString = await message.Content.ReadAsStringAsync();
            } catch (Exception e) {
                Logger.Warning($"Could not get successful string from http response message because message content read as string attempt failed: {e}.");
                return null;
            }

            return successfulString;
        }
        public static async Task<string?> GetSuccessfulString(Task<HttpResponseMessage?> messageTask) => await GetSuccessfulString(await messageTask);
    }
}
