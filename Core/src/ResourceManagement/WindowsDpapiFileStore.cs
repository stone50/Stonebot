#if WINDOWS
namespace StonebotCore.ResourceManagement {
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    internal sealed class WindowsDpapiFileStore : IProtectedFileStore {
        public void Save(string data, string filePath) {
            var plaintext = Encoding.UTF8.GetBytes(data);
            var encrypted = ProtectedData.Protect(plaintext, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(filePath, encrypted);
        }

        public string Load(string filePath) {
            var encrypted = File.ReadAllBytes(filePath);
            var decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
#endif
