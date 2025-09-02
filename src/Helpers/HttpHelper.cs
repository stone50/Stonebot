namespace Stonebot.Helpers {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Web;

    internal static class HttpHelper {
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

        public static T SendAuthorizedGetRequest<T>(string url, JsonTypeInfo<T> jsonTypeInfo) where T : struct => SendGetRequest(Cache.GetAuthorizedHttpClient(), url, jsonTypeInfo);

        public static T SendGetRequest<T>(HttpClient client, string url, JsonTypeInfo<T> jsonTypeInfo) where T : struct {
            var cancellationToken = TaskHelper.GetDefaultCancellationToken();
            var getTask = client.GetAsync(url, cancellationToken);
            var response = TaskHelper.Sync(getTask).EnsureSuccessStatusCode();
            return GetMessageContentAs(response, jsonTypeInfo);
        }

        public static T SendUnauthorizedPostRequest<T>(string url, JsonTypeInfo<T> responseJsonTypeInfo) where T : struct => SendPostRequest(Cache.DefaultHttpClient, url, responseJsonTypeInfo);

        public static T SendAuthorizedPostRequest<T>(string url, JsonTypeInfo<T> responseJsonTypeInfo) where T : struct => SendPostRequest(Cache.GetAuthorizedHttpClient(), url, responseJsonTypeInfo);

        public static T SendPostRequest<T>(HttpClient client, string url, JsonTypeInfo<T> responseJsonTypeInfo) where T : struct {
            var cancellationToken = TaskHelper.GetDefaultCancellationToken();
            var postTask = client.PostAsync(url, null, cancellationToken);
            var response = TaskHelper.Sync(postTask).EnsureSuccessStatusCode();
            return GetMessageContentAs(response, responseJsonTypeInfo);
        }

        public static void SendUnauthorizedPostRequest<T>(string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo) where T : struct => SendPostRequest(Cache.DefaultHttpClient, url, body, bodyJsonTypeInfo);

        public static void SendAuthorizedPostRequest<T>(string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo, CancellationToken? cancellationToken = null) where T : struct => SendPostRequest(Cache.GetAuthorizedHttpClient(), url, body, bodyJsonTypeInfo, cancellationToken);

        public static void SendPostRequest<T>(HttpClient client, string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo, CancellationToken? cancellationToken = null) where T : struct => InnerSendPostRequest(client, url, body, bodyJsonTypeInfo, cancellationToken);

        public static TResponse SendUnauthorizedPostRequest<TBody, TResponse>(string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo) where TBody : struct where TResponse : struct => SendPostRequest(Cache.DefaultHttpClient, url, body, bodyJsonTypeInfo, responseJsonTypeInfo);

        public static TResponse SendAuthorizedPostRequest<TBody, TResponse>(string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo) where TBody : struct where TResponse : struct => SendPostRequest(Cache.GetAuthorizedHttpClient(), url, body, bodyJsonTypeInfo, responseJsonTypeInfo);

        public static TResponse SendPostRequest<TBody, TResponse>(HttpClient client, string url, TBody body, JsonTypeInfo<TBody> bodyJsonTypeInfo, JsonTypeInfo<TResponse> responseJsonTypeInfo) where TBody : struct where TResponse : struct {
            var response = InnerSendPostRequest(client, url, body, bodyJsonTypeInfo);
            return GetMessageContentAs(response, responseJsonTypeInfo);
        }

        public static T GetMessageContentAs<T>(HttpResponseMessage message, JsonTypeInfo<T> jsonTypeInfo) where T : struct {
            var cancellationToken = TaskHelper.GetDefaultCancellationToken();
            var messageContentStream = message.Content.ReadAsStream(cancellationToken);
            return JsonSerializer.Deserialize(messageContentStream, jsonTypeInfo);
        }

        private static HttpResponseMessage InnerSendPostRequest<T>(HttpClient client, string url, T body, JsonTypeInfo<T> bodyJsonTypeInfo, CancellationToken? cancellationToken = null) where T : struct {
            var contentString = JsonSerializer.Serialize(body, bodyJsonTypeInfo);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            var linkedCancellationToken = cancellationToken == null ? TaskHelper.GetDefaultCancellationToken() : CancellationTokenSource.CreateLinkedTokenSource((CancellationToken)cancellationToken, TaskHelper.GetDefaultCancellationToken()).Token;
            var postTask = client.PostAsync(url, content, linkedCancellationToken);
            return TaskHelper.Sync(postTask).EnsureSuccessStatusCode();
        }
    }
}
