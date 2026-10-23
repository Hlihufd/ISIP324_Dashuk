using System;
using System.Collections.Generic;
using System.Linq;

namespace ISIP324_Dashuk
{
    internal static class OrderService
    {
        // Одна строка будущего заказа: товар и сколько штук
        private class OrderLine
        {
            public Product Product;
            public int Quantity;
        }

        // Купить один товар
        public static void BuyOne(User user)
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

            var lines = new List<OrderLine> { new OrderLine { Product = product, Quantity = quantity } };
            CreateOrder(user, lines, false);
        }

        // Купить всю корзину
        public static void BuyCart(User user)
        {
            var cartItems = Core.Context.CartItem.Where(c => c.UserId == user.Id).ToList();
            if (cartItems.Count == 0)
            {
                Console.WriteLine("Корзина пуста");
                return;
            }
            var lines = cartItems.Select(c => new OrderLine { Product = c.Product, Quantity = c.Quantity }).ToList();
            CreateOrder(user, lines, true);
        }

        // Общая часть покупки: проверка склада, выбор ПВЗ, создание заказа
        private static void CreateOrder(User user, List<OrderLine> lines, bool clearCart)
        {
            foreach (var line in lines)
            {
                if (line.Quantity > line.Product.StockQuantity)
                {
                    Console.WriteLine($"Товара {line.Product.Name} недостаточно на складе");
                    return;
                }
            }

            int pickupPointId = ChoosePickupPoint();
            if (pickupPointId == 0) return;

            var order = new Order { UserId = user.Id, PickupPointId = pickupPointId, CreatedAt = DateTime.Now };
            Core.Context.Order.Add(order);
            foreach (var line in lines)
            {
                // цену сохраняем в заказе, чтобы история не менялась при смене цены товара
                Core.Context.OrderProduct.Add(new OrderProduct
                {
                    Order = order,
                    ProductId = line.Product.Id,
                    Quantity = line.Quantity,
                    Price = line.Product.Price
                });
                line.Product.StockQuantity -= line.Quantity;
            }
            if (clearCart)
            {
                var cartItems = Core.Context.CartItem.Where(c => c.UserId == user.Id).ToList();
                Core.Context.CartItem.RemoveRange(cartItems);
            }

            if (Core.TrySave()) Console.WriteLine("Заказ оформлен!");
        }

        // Выбор ПВЗ, возвращает Id или 0, если не выбран
        private static int ChoosePickupPoint()
        {
            var points = Core.Context.PickupPoint.ToList();
            if (points.Count == 0)
            {
                Console.WriteLine("Нет доступных ПВЗ");
                return 0;
            }
            Console.WriteLine("Пункты выдачи:");
            foreach (var point in points)
                Console.WriteLine($"{point.Id}. {point.Name}, {point.Address}");

            int id = ConsoleHelper.ReadInt("Введите ID ПВЗ: ");
            if (!points.Any(p => p.Id == id))
            {
                Console.WriteLine("Такого ПВЗ нет");
                return 0;
            }
            return id;
        }

        // История заказов с сортировкой по дате
        public static void ViewOrders(User user)
        {
            Console.WriteLine("1 - Сначала новые\n2 - Сначала старые");
            int choice = ConsoleHelper.ReadInt("Выбор: ");

            var orders = Core.Context.Order.Where(o => o.UserId == user.Id);
            if (choice == 2) orders = orders.OrderBy(o => o.CreatedAt);
            else orders = orders.OrderByDescending(o => o.CreatedAt);

            var list = orders.ToList();
            if (list.Count == 0)
            {
                Console.WriteLine("Заказов пока нет");
                return;
            }
            foreach (var order in list)
            {
                Console.WriteLine($"\nЗаказ №{order.Id} от {order.CreatedAt}, ПВЗ: {order.PickupPoint.Address}");
                var products = Core.Context.OrderProduct.Where(op => op.OrderId == order.Id).ToList();
                foreach (var op in products)
                    Console.WriteLine($"  {op.Product.Name} x {op.Quantity} по {op.Price}");
            }
        }
    }
}