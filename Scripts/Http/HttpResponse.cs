namespace StoneBot.Scripts.Http {
    using Godot;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Environment = System.Environment;

    internal class HttpResponse {
        public int StatusCode = 200;
        public string ReasonPhrase = "OK";
        public string? Message;

        public async Task<bool> SendToStream(Stream stream) {
            var responseBytes = ToBytes();
            if (responseBytes is null) {
                GD.PushWarning($"Cannot send request to stream because ToBytes failed.");
                return false;
            }

            try {
                await stream.WriteAsync(responseBytes);
            } catch (Exception e) {
                GD.PushWarning($"Could not write response bytes to stream: {e}.");
                return false;
            }

            return true;
        }

        public byte[]? ToBytes() {
            var responseString = $"HTTP/1.1 {StatusCode} {ReasonPhrase}{Environment.NewLine}";

            // TODO: include headers

            responseString += Environment.NewLine;

            if (Message is not null) {
                responseString += Message;
            }

            byte[] responseBytes;
            try {
                responseBytes = Encoding.Default.GetBytes(responseString);
            } catch (Exception e) {
                GD.PushWarning($"Could not get bytes from response string: {e}.");
                return null;
            }

            return responseBytes;
        }
    }
}
