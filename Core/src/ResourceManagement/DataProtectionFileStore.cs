#if !WINDOWS
namespace StonebotCore.ResourceManagement {
    using Microsoft.AspNetCore.DataProtection;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class DataProtectionFileStore : IProtectedFileStore {
        private readonly IDataProtector _protector;

        internal DataProtectionFileStore() {
            var keyDirPath = Path.Join(ResourceManager.StonebotDataDirPath, "keys");
            var keyDirInfo = Directory.CreateDirectory(keyDirPath);
            var provider = DataProtectionProvider.Create(keyDirInfo, opts => opts.SetApplicationName("Stonebot"));
            _protector = provider.CreateProtector("Stonebot.Credentials.v1");
        }

        public Task SaveAsync(string filePath, string data, CancellationToken cancellationToken) {
            var encrypted = _protector.Protect(Encoding.UTF8.GetBytes(data));
            return File.WriteAllBytesAsync(filePath, encrypted, cancellationToken);
        }

        public async Task<string> LoadAsync(string filePath, CancellationToken cancellationToken) {
            var encrypted = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
            var decrypted = _protector.Unprotect(encrypted);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
#endif
