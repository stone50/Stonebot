#if !WINDOWS
namespace StonebotCore.ResourceManagement {
    using Microsoft.AspNetCore.DataProtection;
    using System.IO;
    using System.Text;

    internal sealed class DataProtectionFileStore : IProtectedFileStore {
        private readonly IDataProtector _protector;

        internal DataProtectionFileStore() {
            var keyDirInfo = Directory.CreateDirectory(Path.Join(ResourceManager.AppDataDirPath, "keys"));
            var provider = DataProtectionProvider.Create(keyDirInfo, builder => builder.SetApplicationName("Stonebot"));
            _protector = provider.CreateProtector("Credentials");
        }

        public void Save(string data, string filePath) {
            var plaintext = Encoding.UTF8.GetBytes(data);
            var encrypted = _protector.Protect(plaintext);
            File.WriteAllBytes(filePath, encrypted);
        }

        public string Load(string filePath) {
            var encrypted = File.ReadAllBytes(filePath);
            var decrypted = _protector.Unprotect(encrypted);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
#endif
