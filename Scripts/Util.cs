namespace StoneBot.Scripts {
    using Godot;
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal static class Util {
        public static async Task<T?> ProcessHttpResponseMessage<T>(HttpResponseMessage? response) where T : struct {
            var responseString = await VerifyHttpResponseMessage(response);
            if (responseString is null) {
                GD.PushWarning("Could not process http response message because VerifyHttpResponseMessage failed.");
                return null;
            }

            T responseStruct;
            try {
                responseStruct = JsonSerializer.Deserialize<T>(responseString);
            } catch (Exception e) {
                GD.PushWarning($"Could not parse response json: {e}.");
                return null;
            }

            return responseStruct;
        }

        public static async Task<string?> VerifyHttpResponseMessage(HttpResponseMessage? response) {
            if (response is null) {
                GD.PushWarning("Cannot process http response message because request failed.");
                return null;
            }

            string responseString;
            try {
                responseString = await response.Content.ReadAsStringAsync();
            } catch (Exception e) {
                GD.PushWarning($"Could not read response string: {e}.");
                return null;
            }

            if (!response.IsSuccessStatusCode) {
                GD.PushWarning($"Cannot process http response message because request failed: {responseString}.");
                return null;
            }

            return responseString;
        }
    }
}
