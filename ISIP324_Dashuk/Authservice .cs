using System;
using System.Linq;

namespace ISIP324_Dashuk
{
    internal static class AuthService
    {
        public static void Register()
        {
            string login = ConsoleHelper.ReadText("Введите логин: ", 50);
            if (Core.Context.User.Any(u => u.Login == login))
            {
                Console.WriteLine("Такой логин уже занят");
                return;
            }

            Console.Write("Введите пароль (минимум 4 символа): ");
            string password = Console.ReadLine() ?? "";
            if (password.Length < 4)
            {
                Console.WriteLine("Пароль слишком короткий");
                return;
            }
            Console.Write("Повторите пароль: ");
            if (password != Console.ReadLine())
            {
                Console.WriteLine("Пароли не совпадают");
                return;
            }

            Core.Context.User.Add(new User
            {
                Login = login,
                PasswordHash = PasswordHelper.Hash(password),
                CreatedAt = DateTime.Now
            });
            if (Core.TrySave()) Console.WriteLine($"Аккаунт {login} зарегистрирован. Теперь войдите.");
        }

        // Возвращает пользователя или null, если вход не удался
        public static User Login()
        {
            string login = ConsoleHelper.ReadText("Введите логин: ", 50);
            Console.Write("Введите пароль: ");
            string passwordHash = PasswordHelper.Hash(Console.ReadLine() ?? "");

            var user = Core.Context.User.FirstOrDefault(u => u.Login == login && u.PasswordHash == passwordHash);
            if (user == null) Console.WriteLine("Неверный логин или пароль");
            else Console.WriteLine($"Добро пожаловать, {login}!");
            return user;
        }
    }
}