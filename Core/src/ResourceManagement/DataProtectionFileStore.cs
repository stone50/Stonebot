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
            var provider = DataProtectionProvider.Create(
                keyDirectory: keyDirInfo,
                setupAction: builder => builder.SetApplicationName("Stonebot")
            );
            _protector = provider.CreateProtector("Credentials");
        }

        public Task SaveAsync(
            string filePath,
            string data,
            CancellationToken cancellationToken
        ) {
            var plaintext = Encoding.UTF8.GetBytes(data);
            var encrypted = _protector.Protect(plaintext);
            return File.WriteAllBytesAsync(
                path: filePath,
                bytes: encrypted,
                cancellationToken
            );
        }

        public async Task<string> LoadAsync(
            string filePath,
            CancellationToken cancellationToken
        ) {
            var encrypted = await File.ReadAllBytesAsync(
                path: filePath,
                cancellationToken
            );
            var decrypted = _protector.Unprotect(encrypted);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
#endif
