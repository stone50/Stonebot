namespace StoneBot.Scripts {
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
            try {
                Server.Start();
            } catch (Exception e) {
                GD.PushWarning($"Could not start server: {e}.");
                return false;
            }

            IsRunning = true;

            var success = StartServerRequestHandler();
            if (!success) {
                GD.PushWarning("Cannot start server because StartServerRequestHandler failed.");
                _ = Stop();
                return false;
            }

            return true;
        }

        public bool Stop() {
            try {
                Server.Stop();
            } catch (Exception e) {
                GD.PushWarning($"Could not stop server: {e}.");
                return false;
            }

            IsRunning = false;
            return true;
        }

        private bool StartServerRequestHandler() {
            if (Configuration.ServerMaxRetries is null) {
                GD.PushWarning("Cannot start server request handler because ServerMaxRetries is null.");
                return false;
            }

            try {
                _ = Task.Run(ServerRequestHandler);
            } catch (Exception e) {
                GD.PushWarning($"Could not start server request handler: {e}.");
                return false;
            }

            return true;
        }

        private async Task ServerRequestHandler() {
            var tries = 0;
            while (IsRunning) {
                if (tries > Configuration.ServerMaxRetries) {
                    GD.PushWarning("Cannot accept client because max retries is reached.");
                    _ = Stop();
                    return;
                }

                TcpClient client;
                try {
                    client = await Server.AcceptTcpClientAsync();
                } catch (Exception e) {
                    GD.PushWarning($"Could not accept client: {e}.");
                    tries++;
                    continue;
                }

                tries = 0;

                try {
                    using var stream = client.GetStream();

                    var request = await HttpRequest.FromStream(stream);
                    if (request is null) {
                        GD.PushWarning("Cannot handle request because GetServerRequest failed.");
                        continue;
                    }

                    var handlerArgs = new HttpRequestHandlerArgs(request);
                    RequestHandler.Invoke(this, handlerArgs);

                    var success = await handlerArgs.Response.SendToStream(stream);
                    if (!success) {
                        GD.PushWarning("Cannot handle request because Response.SendToStream failed.");
                        continue;
                    }
                } catch (Exception e) {
                    GD.PushWarning($"Could not get client stream: {e}.");
                    continue;
                }
            }
        }
    }
}
