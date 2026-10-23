using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIP324_Dashuk
{

    internal class Program
    {
      
        static void Main(string[] args)
        {
            User currentuser = null;
            bool loggedIn = false;
            void RegisterUser()
            {
                Console.WriteLine("введите логин");
                string login = Console.ReadLine();
                Console.WriteLine("введите праоль");
                string password = Console.ReadLine();
                Console.WriteLine("повторите пароль");
                while (true)
                {
                    if (password != Console.ReadLine()) Console.WriteLine("Пароли не совпадают");
                    else break;
                }
                Core.Context.User.Add(new User { Login = login, PasswordHash = password });
                Core.Context.SaveChanges();
                Console.WriteLine($"аккаунт {login} зарегестрирован");
            }
            
            void LoginUser()
            {
                Console.Write("dведите логин");
                string login = Console.ReadLine();
                Console.Write("Введите пароль");
                string password = Console.ReadLine();
                var user = Core.Context.User.FirstOrDefault(u => u.Login == login && u.PasswordHash == password);
                if (user != null)
                {
                    Console.WriteLine($"Добро пожаловать, {login}!");
                    currentuser = user;
                    loggedIn = true;

                }
                else Console.WriteLine("Неверный логин или пароль.");
            }
        }
        void ViewProducts()
        {
            var products = Core.Context.Product.ToList();
            Console.WriteLine("Список товаров:");
            foreach (var product in products) Console.WriteLine($"ID: {product.Id}, Название: {product.Name}, Цена: {product.Price}")
        }
        void AddToCart()
        {
            Console.WriteLine("Введи ID товара для добавления в корзину");
            int productID = Console.ReadLine();
            var product = Core.Context.Product.FindDirstOrDefault(f => f.ID == productID);
        }
    }
}
