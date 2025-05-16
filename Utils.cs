namespace Stonebot {
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    internal static class Utils {
        public static string GetUrl(string baseUrl, Dictionary<string, string> queryParams) {
            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString("");
            foreach (var param in queryParams) {
                query[param.Key] = param.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.AbsoluteUri;
        }

        public static async Task<T> SendGetRequestAsync<T>(HttpClient client, string url, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            return await GetMessageContentAsAsync<T>(response, jsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<T> SendPostRequestAsync<T>(HttpClient client, string url, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var response = await client.PostAsync(url, null, cancellationToken).ConfigureAwait(false);
            return await GetMessageContentAsAsync<T>(response, jsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<TResponse> SendPostRequestAsync<TBody, TResponse>(HttpClient client, string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo, CancellationToken cancellationToken) where TResponse : struct {
            var contentString = JsonSerializer.Serialize(body, bodyJsonTypeInfo);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            return await GetMessageContentAsAsync(response, responseJsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<T> GetMessageContentAsAsync<T>(HttpResponseMessage message, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var messageContentStream = await message.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync(messageContentStream, jsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        public static void Exit(Constants.ExitCode exitCode) => Environment.Exit((int)exitCode);
    }
}
