using System.Security.Cryptography;
using System.Text;

namespace RegistryApp.Utils.Helpers {
    public static class StringExtension {
        public static string GetHashSHA256(this string source) {
            var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(source));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            sha256.Dispose();

            return hash;
        }
    }
}
