namespace StonebotDaemon.Services {
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Logging;
    using StonebotDaemon.Models;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class SecretService(IDataProtectionProvider provider, ILogger<SecretService> logger) {
        private readonly IDataProtector _protector = provider.CreateProtector("StonebotDaemon.Secrets.v1");
        private readonly ILogger<SecretService> _logger = logger;
        private readonly string _secretsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot", "secrets.bin");

        internal async Task<Secrets> LoadSecretsAsync(CancellationToken cancellationToken) {
            if (!File.Exists(_secretsPath)) {
                return new();
            }

            var encryptedBytes = await File.ReadAllBytesAsync(_secretsPath, cancellationToken).ConfigureAwait(false);
            var jsonBytes = _protector.Unprotect(encryptedBytes);
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);
            return JsonSerializer.Deserialize<Secrets>(json) ?? new();
        }

        internal Task SaveSecretsAsync(Secrets secrets, CancellationToken cancellationToken) {
            var json = JsonSerializer.Serialize(secrets);
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
            var encryptedBytes = _protector.Protect(jsonBytes);
            return File.WriteAllBytesAsync(_secretsPath, encryptedBytes, cancellationToken);
        }
    }
}