using System;
using System.Linq;

namespace ISIP324_Dashuk
{
    internal static class ProductService
    {
        public static void ViewAll()
        {
            var products = Core.Context.Product.ToList();
            Console.WriteLine("Список товаров:");
            foreach (var product in products)
                Console.WriteLine($"ID: {product.Id,-3} {product.Name,-20} Цена: {product.Price,9} В наличии: {product.StockQuantity}");
        }
    }
}