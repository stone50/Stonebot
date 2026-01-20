namespace StonebotCLI.Commands {
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class Utils {
        internal static Task<Error?> SendGetRequestAsync(HttpClient client, int port, string path, CancellationToken cancellationToken) =>
             SendRequestAsync(HttpMethod.Get, client, port, path, null, cancellationToken);

        internal static Task<Error?> SendPostRequestAsync(HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) =>
             SendRequestAsync(HttpMethod.Post, client, port, path, bodyContent, cancellationToken);

        internal static Task<Error?> SendPatchRequestAsync(HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) =>
            SendRequestAsync(HttpMethod.Patch, client, port, path, bodyContent, cancellationToken);

        internal static async Task<Error?> SendRequestAsync(HttpMethod method, HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) {
            var request = new HttpRequestMessage(method, $"http://localhost:{port}{path}");
            if (bodyContent != null) {
                request.Content = new StringContent(
                    bodyContent,
                    Encoding.UTF8,
                    "application/json"
                );
            }

            Console.WriteLine($"{method} {request.RequestUri?.LocalPath}");
            HttpResponseMessage response;
            try {
                response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            } catch (Exception e) {
                return new(ErrorCode.CommandExecutionFailed, e.Message);
            }

            string responseContent;
            try {
                responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            } catch (Exception e) {
                return new(ErrorCode.CommandExecutionFailed, e.Message);
            }

            if (!response.IsSuccessStatusCode) {
                return new(ErrorCode.CommandExecutionFailed, $"{response.StatusCode} {response.ReasonPhrase}: {responseContent}");
            }

            Console.WriteLine(responseContent);
            return null;
        }
    }
}
