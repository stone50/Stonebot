namespace StoneBot.Scripts {
    using Godot;
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    internal class HttpRequest {
        public enum RequestMethod {
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            CONNECT,
            OPTIONS,
            TRACE
        }

        public RequestMethod Method;
        public string URI = null!;

        public static async Task<HttpRequest?> FromStream(NetworkStream stream) {
            var buffer = new byte[65536];

            int numBytesRead;
            try {
                numBytesRead = await stream.ReadAsync(buffer);
            } catch (Exception e) {
                GD.PushWarning($"Could not read request from client stream: {e}.");
                return null;
            }

            string requestString;
            try {
                requestString = Encoding.Default.GetString(buffer, 0, numBytesRead);
            } catch (Exception e) {
                GD.PushWarning($"Could not get string from client stream bytes: {e}.");
                return null;
            }

            HttpRequest request;
            try {
                request = Parse(requestString);
            } catch (Exception e) {
                GD.PushWarning($"Could not get string from client stream bytes: {e}.");
                return null;
            }

            return request;
        }

        public static HttpRequest Parse(string request) {
            var result = new HttpRequest();

            var requestLines = request.Split(System.Environment.NewLine);
            if (requestLines.Length == 0) {
                throw new ArgumentException("Could not parse request lines.");
            }

            var requestLineParts = requestLines[0].Split(' ');
            if (requestLineParts.Length != 3) {
                throw new ArgumentException("Request line must have 3 parts, separated by spaces.");
            }

            result.Method = GetMethod(requestLineParts[0]);
            result.URI = requestLineParts[1];

            return requestLineParts[2] != "HTTP/1.1"
                ? throw new ArgumentException($"Protocol version '{requestLineParts[2]}' not supported.")
                : result;

            // TODO: include headers and message body
        }

        private static RequestMethod GetMethod(string methodString) {
            foreach (var requestMethod in Enum.GetValues<RequestMethod>()) {
                if (methodString == requestMethod.ToString()) {
                    return requestMethod;
                }
            }

            throw new ArgumentException($"Request method '{methodString}' not supported.");
        }
    }
}