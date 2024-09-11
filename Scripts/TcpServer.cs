namespace StoneBot.Scripts {
    using Godot;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography;
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

        public async Task<bool> StartAsSecure() {
            if (IsRunning) {
                GD.PushWarning($"Cannot start server because server is already running.");
                return false;
            }

            Certificate ??= await GetCertificate();
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

        private static async Task<X509Certificate?> GetCertificate() {
            string currentDirectory;
            try {
                currentDirectory = Directory.GetCurrentDirectory();
            } catch (Exception e) {
                GD.PushWarning($"Could not get current directory: {e}.");
                return null;
            }

            var certificateDirectory = Path.Join(currentDirectory, "localhost.pfx");

            if (!File.Exists(certificateDirectory)) {
                var potentialCertificate = await CreateCertificate(certificateDirectory);
                if (potentialCertificate is null) {
                    GD.PushWarning("Cannot get certificate because CreateCertificate failed.");
                    return null;
                }

                return potentialCertificate;
            }

            X509Certificate certificate;
            try {
                certificate = new(certificateDirectory);
            } catch (Exception e) {
                GD.PushWarning($"Could not create certificate from file: {e}.");
                return null;
            }

            return certificate;
        }

        private static async Task<X509Certificate?> CreateCertificate(string filePath) {
            var ecdsa = ECDsa.Create();

            CertificateRequest certificateRequest;
            try {
                certificateRequest = new CertificateRequest("cn=localhost", ecdsa, HashAlgorithmName.SHA256);
            } catch (Exception e) {
                GD.PushWarning($"Could not create certification request: {e}.");
                return null;
            }

            DateTimeOffset dateTimeOffset;
            try {
                dateTimeOffset = DateTimeOffset.Now.AddYears(5);
            } catch (Exception e) {
                GD.PushWarning($"Could not get date time offset: {e}.");
                return null;
            }

            X509Certificate2 certificate;
            try {
                certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, dateTimeOffset);
            } catch (Exception e) {
                GD.PushWarning($"Could not create self signed certificate: {e}.");
                return null;
            }

            byte[] pfxExportedBytes;
            try {
                pfxExportedBytes = certificate.Export(X509ContentType.Pfx);
            } catch (Exception e) {
                GD.PushWarning($"Could not export certificate: {e}.");
                return null;
            }

            string pfxFilePath;
            try {
                pfxFilePath = Path.ChangeExtension(filePath, ".pfx");
            } catch (Exception e) {
                GD.PushWarning($"Could not change file extension: {e}.");
                return null;
            }

            try {
                await File.WriteAllBytesAsync(pfxFilePath, pfxExportedBytes);
            } catch (Exception e) {
                GD.PushWarning($"Could not write bytes to pfx file: {e}.");
                return null;
            }

            try {
                certificate = new(pfxExportedBytes);
            } catch (Exception e) {
                GD.PushWarning($"Could not create certificate from exported bytes: {e}.");
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
