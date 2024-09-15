namespace StoneBot.Scripts.Bot_Core.Http {
    using Godot;
    using System;
    using System.IO;
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
        public string Uri = null!;

        public static async Task<HttpRequest?> FromStream(Stream stream) {
            var buffer = new byte[1024];

            int numBytesRead;
            try {
                numBytesRead = await stream.ReadAsync(buffer);
            } catch (Exception e) {
                GD.PushWarning($"Could not read request from client stream: {e}.");
                return null;
            }

            var request = FromBuffer(buffer, numBytesRead);
            if (request is null) {
                GD.PushWarning("Cannot parse http request because FromBuffer failed.");
                return null;
            }

            return request;
        }

        public static HttpRequest? FromBuffer(byte[] buffer, int numBytes) {
            string requestString;
            try {
                requestString = Encoding.Default.GetString(buffer, 0, numBytes);
            } catch (Exception e) {
                GD.PushWarning($"Could not get string from client stream bytes: {e}.");
                return null;
            }

            var request = Parse(requestString);
            if (request is null) {
                GD.PushWarning($"Cannot parse http request because Parse failed.");
                return null;
            }

            return request;
        }

        public static HttpRequest? Parse(string request) {
            var result = new HttpRequest();

            var requestLines = request.Split(System.Environment.NewLine);
            if (requestLines.Length == 0) {
                GD.PushWarning("Could not parse request lines.");
                return null;
            }

            var requestLineParts = requestLines[0].Split(' ');
            if (requestLineParts.Length != 3) {
                GD.PushWarning("Cannot parse request string because the top line must have 3 parts, separated by spaces.");
                return null;
            }

            result.Method = GetMethod(requestLineParts[0]);
            result.Uri = requestLineParts[1];

            if (requestLineParts[2] != "HTTP/1.1") {
                GD.PushWarning($"Cannot parse request string because protocol version '{requestLineParts[2]}' is not supported.");
                return null;
            }

            // TODO: include headers and message body

            return result;
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