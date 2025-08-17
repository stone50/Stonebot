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

        public static T SendUnauthorizedGetRequest<T>(string url, JsonTypeInfo<T> jsonTypeInfo) where T : struct => SendGetRequest(Cache.DefaultHttpClient, url, jsonTypeInfo);

        public static T SendAuthorizedGetRequest<T>(string url, JsonTypeInfo<T> jsonTypeInfo) where T : struct => SendGetRequest(Cache.AuthorizedHttpClient, url, jsonTypeInfo);

        public static T SendGetRequest<T>(HttpClient client, string url, JsonTypeInfo<T> jsonTypeInfo) where T : struct {
            var cancellationToken = GetDefaultCancellationToken();
            var getTask = client.GetAsync(url, cancellationToken);
            var response = Sync(getTask);
            return GetMessageContentAs(response, jsonTypeInfo);
        }

        public static T SendUnauthorizedPostRequest<T>(string url, JsonTypeInfo<T> responseJsonTypeInfo) where T : struct => SendPostRequest(Cache.DefaultHttpClient, url, responseJsonTypeInfo);

        public static T SendAuthorizedPostRequest<T>(string url, JsonTypeInfo<T> responseJsonTypeInfo) where T : struct => SendPostRequest(Cache.AuthorizedHttpClient, url, responseJsonTypeInfo);

        public static T SendPostRequest<T>(HttpClient client, string url, JsonTypeInfo<T> responseJsonTypeInfo) where T : struct {
            var cancellationToken = GetDefaultCancellationToken();
            var postTask = client.PostAsync(url, null, cancellationToken);
            var response = Sync(postTask);
            return GetMessageContentAs(response, responseJsonTypeInfo);
        }

        public static void SendUnauthorizedPostRequest<T>(string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo) where T : struct => SendPostRequest(Cache.DefaultHttpClient, url, body, bodyJsonTypeInfo);

        public static void SendAuthorizedPostRequest<T>(string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo) where T : struct => SendPostRequest(Cache.AuthorizedHttpClient, url, body, bodyJsonTypeInfo);

        public static void SendPostRequest<T>(HttpClient client, string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo) where T : struct => InnerSendPostRequest(client, url, body, bodyJsonTypeInfo).EnsureSuccessStatusCode();

        public static TResponse SendUnauthorizedPostRequest<TBody, TResponse>(string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo) where TBody : struct where TResponse : struct => SendPostRequest(Cache.DefaultHttpClient, url, body, bodyJsonTypeInfo, responseJsonTypeInfo);

        public static TResponse SendAuthorizedPostRequest<TBody, TResponse>(string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo) where TBody : struct where TResponse : struct => SendPostRequest(Cache.AuthorizedHttpClient, url, body, bodyJsonTypeInfo, responseJsonTypeInfo);

        public static TResponse SendPostRequest<TBody, TResponse>(HttpClient client, string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo) where TBody : struct where TResponse : struct {
            var response = InnerSendPostRequest(client, url, body, bodyJsonTypeInfo);
            return GetMessageContentAs(response, responseJsonTypeInfo);
        }

        public static T GetMessageContentAs<T>(HttpResponseMessage message, JsonTypeInfo<T> jsonTypeInfo) where T : struct {
            var cancellationToken = GetDefaultCancellationToken();
            var messageContentStream = message.Content.ReadAsStream(cancellationToken);
            return JsonSerializer.Deserialize(messageContentStream, jsonTypeInfo);
        }

        public static CancellationToken GetDefaultCancellationToken() => GetCancellationTokenFromSeconds(Constants.DefaultCancellationTokenTimeoutSecs);

        public static CancellationToken GetCancellationTokenFromSeconds(long seconds) => new CancellationTokenSource(TimeSpan.FromSeconds(seconds)).Token;

        public static Task FireTryElseConsoleErrorAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => TryElseConsoleErrorAfter(action, tasks), cancellationToken);

        public static Task FireTryElseConsoleErrorAfter(Action action, params Task[] tasks) => Task.Run(() => TryElseConsoleErrorAfter(action, tasks));

        public static Task FireTryElseErrorAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => TryElseErrorAfter(action, tasks), cancellationToken);

        public static Task FireTryElseErrorAfter(Action action, params Task[] tasks) => Task.Run(() => TryElseErrorAfter(action, tasks));

        public static Task FireTryElseAfter(Action action, Action<Exception> errorAction, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => TryElseAfter(action, errorAction, tasks), cancellationToken);

        public static Task FireTryElseAfter(Action action, Action<Exception> errorAction, params Task[] tasks) => Task.Run(() => TryElseAfter(action, errorAction, tasks));

        public static void TryElseConsoleErrorAfter(Action action, params Task[] tasks) => TryElseConsoleError(() => DoAfter(action, tasks));

        public static void TryElseErrorAfter(Action action, params Task[] tasks) => TryElseError(() => DoAfter(action, tasks));

        public static void TryElseAfter(Action action, Action<Exception> errorAction, params Task[] tasks) => TryElse(() => DoAfter(action, tasks), errorAction);

        public static Task FireDoAfter(Action action, CancellationToken cancellationToken, params Task[] tasks) => Task.Run(() => DoAfter(action, tasks), cancellationToken);

        public static Task FireDoAfter(Action action, params Task[] tasks) => Task.Run(() => DoAfter(action, tasks));

        public static void DoAfter(Action action, params Task[] tasks) {
            foreach (var task in tasks) {
                Sync(task);
            }

            action();
        }

        public static T Sync<T>(Task<T> task) => task.GetAwaiter().GetResult();

        public static T Sync<T>(ValueTask<T> task) => task.GetAwaiter().GetResult();

        public static void Sync(Task task) => task.GetAwaiter().GetResult();

        public static Task FireTryElseConsoleError(Action action, CancellationToken cancellationToken) => Task.Run(() => TryElseConsoleError(action), cancellationToken);

        public static Task FireTryElseConsoleError(Action action) => Task.Run(() => TryElseConsoleError(action));

        public static Task FireTryElseError(Action action, CancellationToken cancellationToken) => Task.Run(() => TryElseError(action), cancellationToken);

        public static Task FireTryElseError(Action action) => Task.Run(() => TryElseError(action));

        public static Task FireTryElse(Action action, Action<Exception> errorAction, CancellationToken cancellationToken) => Task.Run(() => TryElse(action, errorAction), cancellationToken);

        public static Task FireTryElse(Action action, Action<Exception> errorAction) => Task.Run(() => TryElse(action, errorAction));

        public static void TryElseConsoleError(Action action) => TryElse(action, Console.Error.WriteLine);

        public static void TryElseError(Action action) => TryElse(action, (e) => Logger.Error(e));

        public static void TryElse(Action action, Action<Exception> errorAction) {
            try {
                action();
            } catch (OperationCanceledException) {
            } catch (Exception e) {
                try {
                    errorAction(e);
                } finally { }
            }
        }

        private static HttpResponseMessage InnerSendPostRequest<T>(HttpClient client, string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo) where T : struct {
            var contentString = JsonSerializer.Serialize(body, bodyJsonTypeInfo);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var cancellationToken = GetDefaultCancellationToken();
            var postTask = client.PostAsync(url, content, cancellationToken);
            return Sync(postTask);
        }
    }
}
