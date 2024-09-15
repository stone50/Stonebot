namespace StoneBot.Scripts.Bot_Core.Http {
    using Godot;
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading.Tasks;

    internal class WebSocketClient {
        public delegate void WebSocketRequestHandler(object sender, string request);
        public event WebSocketRequestHandler RequestHandler = delegate { };

        private readonly ClientWebSocket WebSocket = new();

        public bool IsRunning { get; private set; } = false;

        public async Task<bool> Start(string uriString) {
            if (IsRunning) {
                GD.PushWarning($"Cannot start server because server is already running.");
                return false;
            }

            Uri uri;
            try {
                uri = new(uriString);
            } catch (Exception e) {
                GD.PushWarning($"Could not create uri from uri string: {e}.");
                return false;
            }

            try {
                await WebSocket.ConnectAsync(uri, default);
            } catch (Exception e) {
                GD.PushWarning($"Could not start server: {e}.");
                return false;
            }

            IsRunning = true;
            StartListening();

            return true;
        }

        public async Task<bool> Stop(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure, string statusDescription = "Client Closed") {
            if (!IsRunning) {
                GD.PushWarning($"Cannot stop server because server is not running.");
                return false;
            }

            try {
                await WebSocket.CloseAsync(closeStatus, statusDescription, default);
            } catch (Exception e) {
                GD.PushWarning($"Could not stop server: {e}.");
                return false;
            }

            IsRunning = false;
            return true;
        }

        private void StartListening() => Task.Run(async () => {
            while (IsRunning) {
                var resultBytes = new byte[65536];
                WebSocketReceiveResult result;
                try {
                    result = await WebSocket.ReceiveAsync(resultBytes, default);
                } catch (Exception e) {
                    GD.PushWarning($"Could not receive from web socket: {e}.");
                    continue;
                }

                if (result.CloseStatus is not null) {
                    GD.PushWarning($"WebSocketClient closed by server: {result.CloseStatus}: {result.CloseStatusDescription ?? "no description"}.");
                    if (!await Stop()) {
                        GD.PushError("WebSocketClient stop failed.");
                    }

                    return;
                }

                string request;
                try {
                    request = Encoding.Default.GetString(resultBytes, 0, result.Count);
                } catch (Exception e) {
                    GD.PushWarning($"Could not get string from result bytes: {e}.");
                    return;
                }

                RequestHandler.Invoke(this, request);
            }
        });
    }
}
