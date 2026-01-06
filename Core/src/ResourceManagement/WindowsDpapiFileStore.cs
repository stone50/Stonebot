#if WINDOWS
namespace StonebotCore.ResourceManagement {
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class WindowsDpapiFileStore : IProtectedFileStore {
        public Task SaveAsync(string filePath, string data, CancellationToken cancellationToken) {
            var plaintext = Encoding.UTF8.GetBytes(data);
            var encrypted = ProtectedData.Protect(plaintext, null, DataProtectionScope.CurrentUser);
            return File.WriteAllBytesAsync(filePath, encrypted, cancellationToken);
        }

        public async Task<string> LoadAsync(string filePath, CancellationToken cancellationToken) {
            var encrypted = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
            var decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
#endif
