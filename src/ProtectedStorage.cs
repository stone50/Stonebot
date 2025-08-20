namespace Stonebot {
    using Microsoft.AspNetCore.DataProtection;
    using System.Text;

    internal static class ProtectedStorage {
        public static void SaveRefreshToken(string refreshToken) {
            var unecryptedBytes = Encoding.UTF8.GetBytes(refreshToken);
            var encryptedBytes = protector!.Protect(unecryptedBytes);
            File.WriteAllBytes(Constants.RefreshTokenFilePath, encryptedBytes);
        }

        public static string GetRefreshToken() {
            var encryptedBytes = File.ReadAllBytes(Constants.RefreshTokenFilePath);
            var decryptedBytes = protector!.Unprotect(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static readonly IDataProtector? protector = DataProtectionProvider.Create("Stonebot").CreateProtector("RefreshTokenProtector");
    }
}
