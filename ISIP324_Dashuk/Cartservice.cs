using System;
using System.Linq;

namespace ISIP324_Dashuk
{
    internal static class CartService
    {
        public static void Add(User user)
        {
            int productId = ConsoleHelper.ReadInt("Введите ID товара: ");
            var product = Core.Context.Product.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine("Такого товара нет");
                return;
            }
            int quantity = ConsoleHelper.ReadInt("Количество: ");
            if (quantity <= 0)
            {
                Console.WriteLine("Количество должно быть больше 0");
                return;
            }

            // если товар уже в корзине - увеличиваем количество
            var item = Core.Context.CartItem.FirstOrDefault(c => c.UserId == user.Id && c.ProductId == productId);
            int newQuantity = quantity + (item == null ? 0 : item.Quantity);
            if (newQuantity > product.StockQuantity)
            {
                Console.WriteLine("Столько нет на складе");
                return;
            }

            if (item == null)
                Core.Context.CartItem.Add(new CartItem { UserId = user.Id, ProductId = productId, Quantity = quantity });
            else
                item.Quantity = newQuantity;

            if (Core.TrySave()) Console.WriteLine("Товар добавлен в корзину");
        }

        // Показывает корзину, возвращает false если она пуста
        public static bool View(User user)
        {
            var cartItems = Core.Context.CartItem.Where(c => c.UserId == user.Id).ToList();
            if (cartItems.Count == 0)
            {
                Console.WriteLine("Корзина пуста");
                return false;
            }
            decimal total = 0;
            foreach (var item in cartItems)
            {
                decimal sum = item.Product.Price * item.Quantity;
                total += sum;
                Console.WriteLine($"ID: {item.ProductId,-3} {item.Product.Name,-20} {item.Quantity} шт. x {item.Product.Price} = {sum}");
            }
            Console.WriteLine($"Итого: {total}");
            return true;
        }

        public static void ChangeQuantity(User user)
        {
            if (!View(user)) return;
            int productId = ConsoleHelper.ReadInt("Введите ID товара в корзине: ");
            var item = Core.Context.CartItem.FirstOrDefault(c => c.UserId == user.Id && c.ProductId == productId);
            if (item == null)
            {
                Console.WriteLine("Такого товара в корзине нет");
                return;
            }
            int quantity = ConsoleHelper.ReadInt("Новое количество (0 - удалить): ");
            if (quantity < 0)
            {
                Console.WriteLine("Количество не может быть отрицательным");
                return;
            }
            if (quantity > item.Product.StockQuantity)
            {
                Console.WriteLine("Столько нет на складе");
                return;
            }

            if (quantity == 0) Core.Context.CartItem.Remove(item);
            else item.Quantity = quantity;

            if (Core.TrySave()) Console.WriteLine("Корзина обновлена");
        }

        public static void Remove(User user)
        {
            if (!View(user)) return;
            int productId = ConsoleHelper.ReadInt("Введите ID товара для удаления: ");
            var item = Core.Context.CartItem.FirstOrDefault(c => c.UserId == user.Id && c.ProductId == productId);
            if (item == null)
            {
                Console.WriteLine("Такого товара в корзине нет");
                return;
            }
            Core.Context.CartItem.Remove(item);
            if (Core.TrySave()) Console.WriteLine("Товар удалён из корзины");
        }
    }
}