
using System.Security.Cryptography;
using System.Text;

namespace MyWordStystemWebapi.Helpers
{
    //密码哈希算法类
    public static class SecurityHelper
    {
        public static string GenerateHash(string password, string salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt)))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)); // 256位哈希值
            }
        }

        public static bool VerifyPasswordHash(string password, string storedHash, string salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt)))
            {
                var hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
                return hash == storedHash;
            }
        }
    }
}
