using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Marketplays
{
    // 1. Модель пользователя
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    // 2. Сервис для работы с пользователями и файлом
    public class UserService
    {
        private readonly string _filePath = "users.json";
        private List<User> _users = new List<User>();

        public UserService()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (!File.Exists(_filePath))
            {
                return; // Файла нет, список останется пустым
            }

            try
            {
                var json = File.ReadAllText(_filePath);

                // Если файл пустой, создаем новый список
                if (string.IsNullOrWhiteSpace(json))
                {
                    _users = new List<User>;
                    return;
                }
                //десериализуем json обратно в список
                _users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>;
            } catch (Exception ex)
            {
                Console.WriteLine($"Ошибка!!!!!!!! Не удалось загрузить факин данныу: {ex.ToString())}";
                _users = new List<User>;
            }
        }
        private void SaveUsers()
        {
            try
            {
                // Сериализуем список в красивый JSON с отступами
                var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка!!!!!!!! Не удалось загрузить факин данныу: {ex.ToString())}";
            }
        }

        public bool Registr(string username, string password)
        {
            // Проверяем, нет ли уже такого пользователя 
            if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnorCase))) return false;
            _users.Add(new User(username, password));
            SaveUsers();
            return true;
        }

        public bool Authenticate(string usermame, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username.Equals(usermame, StringComparison.OrdinalIgnorCase));
            // Если пользователь найден и пароли совпадают
            if (user != null && user.Password == password) { return true} return false;
        }
    }
    internal class Program
    {

        static void Main(string[] args)
        {

        }
    }
}