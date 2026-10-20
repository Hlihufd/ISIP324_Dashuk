using System;
using System.Security.Cryptography;

namespace ISIP324_Dashuk
{
    // Пароль в БД хранится не в открытом виде, а как "соль.хеш" (оба значения в Base64).
    // Соль - случайные байты, из-за них одинаковые пароли дают разные хеши.
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string Hash(string password) {
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt) + "." +
                Convert.ToBase64String(Derive(password, salt));
        }

        public static bool Verify(string password, string stored) {
            if (string.IsNullOrEmpty(stored)) return false;

            var parts = stored.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[0]);
                expected = Convert.FromBase64String(parts[1]);
            }
            catch (FormatException)
            {
                return false;
            }

            return AreEqual(expected, Derive(password, salt));
        }

        private static byte[] Derive(string password, byte[] salt) {
            // Конструктор с HashAlgorithmName 4.7.2
            using(var kfg = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return kfg.GetBytes(HashSize);
            }
        }

        // Сравнение без раннего выхода, чтобы время проверки не зависело от места первого отличия
        public static bool AreEqual(byte[] a, byte[] b)
        {
            if(a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}