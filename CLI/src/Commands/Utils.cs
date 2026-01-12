namespace StonebotCLI.Commands {
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    internal static class Utils {
        internal static Task<HttpResponseMessage> SendGetRequestAsync(HttpClient client, int port, string path) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:{port}{path}");
            return client.SendAsync(request);
        }

        internal static Task<HttpResponseMessage> SendPostRequestAsync(HttpClient client, int port, string path, string? bodyContent = null) {
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:{port}{path}");
            if (bodyContent != null) {
                request.Content = new StringContent(
                    bodyContent,
                    Encoding.UTF8,
                    "application/json"
                );
            }

            return client.SendAsync(request);
        }
    }
}
