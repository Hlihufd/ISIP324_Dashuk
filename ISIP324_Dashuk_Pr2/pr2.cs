using System;
using System.Collections.Generic;
using System.Linq;

namespace Pr_2
{
    // 1. Категории товаров 
    public enum ProductCategory
    {
        Electronic = 1,
        Food,
        Clothing
    }

    // 2. Класс Товара
    public class Product
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ProductCategory Category { get; set; }
        public bool IsInStock => Quantity > 0;

        public Product(int id, string name, decimal price, int quantity, ProductCategory category) 
        { 
            Id = id;
            Name = name;    
            Price = price;
            Category = category;
            Quantity = quantity;
        }

        public override string ToString() { 
            strinng stockStatus = IsInStock ? $"В наличии ({Quantity} шт.)" : "НЕТ НА СКЛАДЕ";
            return $"[Код: {Id}] {Name,-18} | Категория: {Category,-11} | Цена: {Price,8:C} | Статус: {stockStatus}";
        }

        // 3. Запись о продаже
        public class SaleRecord
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public int QuantitySold { get; set; }
            public decimal TotalPrice { get; set; }
            public DataTime SaleTime { get; set; }

            public SaleRecord(int productID, string productName, int quantitySold, decimal totalPrice)
            {
                ProductID = productID;
                ProductName = productName;
                QuantitySold = quantitySold;
                TotalPrice = totalPrice;
                SaleTime = DataTime.Now;
            }

            public override string ToString()
            {
                return $"[{SaleTime:HH:mm:ss}] {ProductName} — {QuantitySold} шт. на сумму {TotalPrice:C}";
            }
        }

        class Program
        {
            // Генератор уникального кода
            private static int _nextID = 1001;

            // Основной список товаров
            private static List<Product> _products = new List<Product>();

            // Стек для истории продаж
            private static Stack<SaleRecord> _salesHistory = new Stack<SaleRecord>();

            static void Main(string[] args)
            {
                Console.OutputEncording = System.Text.Encoding.UTF8;
                
                // Заполнение списка 5 тестовыми данными
                SeeData();

                bool isRunning = true;
                while (isRunning) {
                    Console.Clear();
                    Console.WriteLine("        СИСТЕМА УЧЁТА ТОВАРОВ В МАГАЗИНЕ         ");
                    Console.WriteLine("1. Показать список всех товаров");
                    Console.WriteLine("2. Добавить новый товар");
                    Console.WriteLine("3. Удалить товар");
                    Console.WriteLine("4. Заказать поставку товара (пополнить)");
                    Console.WriteLine("5. Продать товар");
                    Console.WriteLine("6. Поиск товаров (по коду, названию, категории)");
                    Console.WriteLine("7. Отменить последнюю продажу");
                    Console.WriteLine("8. Отчёт о продажах");
                    Console.WriteLine("0. Выход");

                    int choice = ReadInt("Выберите действие: ", 0, 8);
                    Console.clear();
                }
            }

        }
    }
}