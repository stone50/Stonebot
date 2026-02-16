namespace StonebotCLI.Commands {
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class Utils {
        private static readonly Stream StandardOutput = Console.OpenStandardOutput();

        internal static Task<Error?> SendGetRequestAsync(HttpClient client, int port, string path, CancellationToken cancellationToken) =>
             SendRequestAsync(HttpMethod.Get, client, port, path, null, cancellationToken);

        internal static Task<Error?> SendPostRequestAsync(HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) =>
             SendRequestAsync(HttpMethod.Post, client, port, path, bodyContent, cancellationToken);

        internal static Task<Error?> SendPatchRequestAsync(HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) =>
            SendRequestAsync(HttpMethod.Patch, client, port, path, bodyContent, cancellationToken);

        internal static async Task<Error?> SendRequestAsync(HttpMethod method, HttpClient client, int port, string path, string? bodyContent, CancellationToken cancellationToken) {
            using var request = new HttpRequestMessage(method, $"http://localhost:{port}{path}");
            if (bodyContent != null) {
                request.Content = new StringContent(
                    bodyContent,
                    Encoding.UTF8,
                    "application/json"
                );
            }

            Console.WriteLine($"{method} {request.RequestUri!.LocalPath}");
            try {
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) {
                    return new(ErrorCode.CommandExecutionFailed, $"{response.ReasonPhrase} | {await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)}");
                }

                await response.Content.CopyToAsync(StandardOutput, cancellationToken).ConfigureAwait(false);
                Console.WriteLine();
            } catch (Exception e) {
                return new(ErrorCode.CommandExecutionFailed, e.Message);
            }

            return null;
        }
    }
}
