namespace StoneBot.Scripts.Bot_Core.Http {
    using Godot;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    internal class TcpServer {
        public class HttpRequestHandlerArgs {
            public HttpRequest Request;
            public HttpResponse Response = new();

            public HttpRequestHandlerArgs(HttpRequest request) => Request = request;
        }

        public delegate void HttpRequestHandler(object sender, HttpRequestHandlerArgs args);
        public event HttpRequestHandler RequestHandler = delegate { };

        private readonly TcpListener Server;

        public bool IsRunning { get; private set; } = false;

        public TcpServer(IPAddress ipAddress, int port) => Server = new TcpListener(ipAddress, port);

        public bool Start() {
            if (IsRunning) {
                GD.PushWarning($"Cannot start server because server is already running.");
                return false;
            }

            try {
                Server.Start();
            } catch (Exception e) {
                GD.PushWarning($"Could not start server: {e}.");
                return false;
            }

            IsRunning = true;
            StartListening();

            return true;
        }

        public bool Stop() {
            if (!IsRunning) {
                GD.PushWarning($"Cannot stop server because server is not running.");
                return false;
            }

            try {
                Server.Stop();
            } catch (Exception e) {
                GD.PushWarning($"Could not stop server: {e}.");
                return false;
            }

            IsRunning = false;
            return true;
        }

        private void StartListening() => Task.Run(async () => {
            while (IsRunning) {
                TcpClient client;
                try {
                    client = await Server.AcceptTcpClientAsync();
                } catch (Exception e) {
                    GD.PushWarning($"Could not accept client: {e}.");
                    return;
                }

                var response = new HttpResponse();
                try {
                    using var clientStream = client.GetStream();

                    var request = await HttpRequest.FromStream(clientStream);
                    if (request is null) {
                        GD.PushWarning("Cannot handle request because HttpRequest.FromStream failed.");
                        response.StatusCode = 400;
                        response.ReasonPhrase = "Bad Request";
                    } else {
                        var handlerArgs = new HttpRequestHandlerArgs(request);
                        RequestHandler.Invoke(this, handlerArgs);
                        response = handlerArgs.Response;
                    }

                    if (!await response.SendToStream(clientStream)) {
                        GD.PushWarning("Cannot handle request because HttpResponse.SendToStream failed.");
                        continue;
                    }
                } catch (Exception e) {
                    GD.PushWarning($"Could not get client stream: {e}.");
                    continue;
                }
            }
        });
    }
}
