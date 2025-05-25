namespace Stonebot {
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
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

        public static T SendGetRequest<T>(HttpClient client, string url, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var getTask = client.GetAsync(url, cancellationToken);
            var response = Sync(getTask);
            return GetMessageContentAs<T>(response, jsonTypeInfo, cancellationToken);
        }
        public static T SendPostRequest<T>(HttpClient client, string url, JsonTypeInfo<T> responseJsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var postTask = client.PostAsync(url, null, cancellationToken);
            var response = Sync(postTask);
            return GetMessageContentAs(response, responseJsonTypeInfo, cancellationToken);
        }

        public static void SendPostRequest<T>(HttpClient client, string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var response = InnerSendPostRequest(client, url, body, bodyJsonTypeInfo, cancellationToken);
            _ = response.EnsureSuccessStatusCode();
        }

        public static T GetMessageContentAs<T>(HttpResponseMessage message, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var messageContentStream = message.Content.ReadAsStream(cancellationToken);
            return JsonSerializer.Deserialize(messageContentStream, jsonTypeInfo);
        }

        public static T Sync<T>(Task<T> task) => task.GetAwaiter().GetResult();

        public static T Sync<T>(ValueTask<T> task) => task.GetAwaiter().GetResult();

        public static void Sync(Task task) => task.GetAwaiter().GetResult();

        public static void TryElseConsoleError(Action action) => TryElseLog(action, Console.Error.WriteLine);

        public static void TryElseWarn(Action action) => TryElseLog(action, (e) => Logger.Warn(e));

        public static void TryElseError(Action action) => TryElseLog(action, (e) => Logger.Error(e));

        public static void TryElseLog(Action action, Action<Exception> log) {
            try {
                action();
            } catch (OperationCanceledException) {
            } catch (Exception e) {
                try {
                    log(e);
                } finally { }
            }
        }

        private static HttpResponseMessage InnerSendPostRequest<T>(HttpClient client, string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var contentString = JsonSerializer.Serialize(body, bodyJsonTypeInfo);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var postTask = client.PostAsync(url, content, cancellationToken);
            return Sync(postTask);
        }
    }
}
