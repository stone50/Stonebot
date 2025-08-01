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

        public static TResponse SendPostRequest<TBody, TResponse>(HttpClient client, string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo, CancellationToken cancellationToken) where TBody : struct where TResponse : struct {
            var response = InnerSendPostRequest(client, url, body, bodyJsonTypeInfo, cancellationToken);
            return GetMessageContentAs(response, responseJsonTypeInfo, cancellationToken);
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

        public static void DoAfter(Action action, params Task[] tasks) {
            foreach (var task in tasks) {
                Sync(task);
            }

            action();
        }

        public static void TryElseErrorAfter(Action action, params Task[] tasks) => DoAfter(() => TryElseError(action), tasks);

        public static Task FireTryElseConsoleError(Action action, CancellationToken cancellationToken) => Task.Run(() => TryElseConsoleError(action), cancellationToken);

        public static Task FireTryElseError(Action action, CancellationToken cancellationToken) => Task.Run(() => TryElseError(action), cancellationToken);

        public static Task FireDoAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => DoAfter(action, tasks), cancellationToken);

        public static Task FireTryElseErrorAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => FireDoAfter(() => TryElseError(action), cancellationToken, tasks);

        private static HttpResponseMessage InnerSendPostRequest<T>(HttpClient client, string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo, CancellationToken cancellationToken) where T : struct {
            var contentString = JsonSerializer.Serialize(body, bodyJsonTypeInfo);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var postTask = client.PostAsync(url, content, cancellationToken);
            return Sync(postTask);
        }
    }
}
