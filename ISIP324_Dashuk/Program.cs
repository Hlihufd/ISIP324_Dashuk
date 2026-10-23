using System;

namespace ISIP324_Dashuk
{
    internal class Program
    {
        // текущий пользователь (null = не вошёл)
        static User currentUser = null;

        static void Main(string[] args)
        {
            if (!Core.CheckConnection()) return;

            while (true)
            {
                Console.WriteLine("\n   MOGDAUN");
                if (currentUser == null)
                {
                    Console.WriteLine("1 - Товары\n2 - Регистрация\n3 - Вход\n0 - Выход");
                    switch (ConsoleHelper.ReadInt("Выбор: "))
                    {
                        case 1: ProductService.ViewAll(); break;
                        case 2: AuthService.Register(); break;
                        case 3: currentUser = AuthService.Login(); break;
                        case 0: return;
                        default: Console.WriteLine("Нет такого пункта"); break;
                    }
                }
                else
                {
                    Console.WriteLine($"Вы вошли как {currentUser.Login}");
                    Console.WriteLine("1 - Товары\n2 - Добавить в корзину\n3 - Корзина\n4 - Изменить количество в корзине\n5 - Удалить из корзины\n6 - Купить один товар\n7 - Купить всю корзину\n8 - Мои заказы\n9 - Выйти из аккаунта\n0 - Выход");
                    switch (ConsoleHelper.ReadInt("Выбор: "))
                    {
                        case 1: ProductService.ViewAll(); break;
                        case 2: CartService.Add(currentUser); break;
                        case 3: CartService.View(currentUser); break;
                        case 4: CartService.ChangeQuantity(currentUser); break;
                        case 5: CartService.Remove(currentUser); break;
                        case 6: OrderService.BuyOne(currentUser); break;
                        case 7: OrderService.BuyCart(currentUser); break;
                        case 8: OrderService.ViewOrders(currentUser); break;
                        case 9: currentUser = null; break;
                        case 0: return;
                        default: Console.WriteLine("Нет такого пункта"); break;
                    }
                }
            }
        }
    }
}