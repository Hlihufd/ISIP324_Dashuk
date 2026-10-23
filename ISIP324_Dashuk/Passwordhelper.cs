using System;
using System.Security.Cryptography;
using System.Text;

namespace ISIP324_Dashuk
{
    internal static class PasswordHelper
    {
        // Превращает пароль в хеш (в базе пароль в открытом виде хранить нельзя)
        public static string Hash(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
    }
}