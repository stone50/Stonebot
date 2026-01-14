namespace StonebotCLI.Commands {
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class Utils {
        internal static Task<HttpResponseMessage> SendGetRequestAsync(HttpClient client, int port, string path, CancellationToken cancellationToken) =>
             SendRequestAsync(HttpMethod.Get, client, port, path, null, cancellationToken);

        internal static Task<HttpResponseMessage> SendPostRequestAsync(HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) =>
             SendRequestAsync(HttpMethod.Post, client, port, path, bodyContent, cancellationToken);

        internal static Task<HttpResponseMessage> SendPatchRequestAsync(HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) =>
            SendRequestAsync(HttpMethod.Patch, client, port, path, bodyContent, cancellationToken);

        internal static async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) {
            var request = new HttpRequestMessage(method, $"http://localhost:{port}{path}");
            if (bodyContent != null) {
                request.Content = new StringContent(
                    bodyContent,
                    Encoding.UTF8,
                    "application/json"
                );
            }

            Console.Write($"{method} {request.RequestUri?.LocalPath}");
            var response = await client.SendAsync(request, cancellationToken);
            Console.Write($": {await response.Content.ReadAsStringAsync(cancellationToken)}");
            return response;
        }
    }
}
