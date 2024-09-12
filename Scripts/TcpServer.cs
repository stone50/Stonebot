namespace StoneBot.Scripts {
    using Godot;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;

    internal class TcpServer {
        public class HttpRequestHandlerArgs {
            public HttpRequest Request;
            public HttpResponse Response = new();

            public HttpRequestHandlerArgs(HttpRequest request) => Request = request;
        }

        public delegate void HttpRequestHandler(object sender, HttpRequestHandlerArgs args);
        public event HttpRequestHandler RequestHandler = delegate { };

        private readonly TcpListener Server;
        private static X509Certificate? Certificate;

        public bool IsRunning { get; private set; } = false;
        public bool IsRunningAsSecure { get; private set; } = false;

        public TcpServer(IPAddress ipAddress, int port) => Server = new TcpListener(ipAddress, port);

        public bool StartAsSecure() {
            if (IsRunning) {
                GD.PushWarning($"Cannot start server because server is already running.");
                return false;
            }

            Certificate ??= GetCertificate();
            if (Certificate is null) {
                GD.PushWarning("Cannot start secure server because GetCertificate failed.");
                return false;
            }

            IsRunningAsSecure = true;
            return Start();
        }

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
            IsRunningAsSecure = false;
            return true;
        }

        private static X509Certificate? GetCertificate() {
            X509Store store;
            try {
                store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            } catch (Exception e) {
                GD.PushWarning($"Could not get certificate store: {e}.");
                return null;
            }

            try {
                store.Open(OpenFlags.ReadOnly);
            } catch (Exception e) {
                GD.PushWarning($"Could not open certificate store: {e}.");
                return null;
            }

            X509Certificate2 certificate;
            try {
                certificate = store.Certificates.First(cert => cert.IssuerName.Name == "CN=localhost");
            } catch (Exception e) {
                GD.PushWarning($"Could not find localhost certificate: {e}.");
                return null;
            }

            return certificate;
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

                var response = new HttpResponse();
                try {
                    using var clientStream = client.GetStream();

                    Stream stream = clientStream;
                    if (IsRunningAsSecure) {
                        if (Certificate is null) {
                            GD.PushWarning("Cannot authenticate client because Certificate is null.");
                            continue;
                        }

                        var sslStream = new SslStream(stream, false);

                        try {
                            await sslStream.AuthenticateAsServerAsync(Certificate, false, true);
                        } catch (Exception e) {
                            GD.PushWarning($"Could not authenticate client: {e}.");
                            continue;
                        }

                        stream = sslStream;
                    }

                    var request = await HttpRequest.FromStream(stream);
                    if (request is null) {
                        GD.PushWarning("Cannot handle request because HttpRequest.FromStream failed.");
                        response.StatusCode = 400;
                        response.ReasonPhrase = "Bad Request";
                    } else {
                        var handlerArgs = new HttpRequestHandlerArgs(request);
                        RequestHandler.Invoke(this, handlerArgs);
                        response = handlerArgs.Response;
                    }

                    var success = await response.SendToStream(stream);
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
