using System;
using System.Collections.Generic;
using System.Linq;

namespace Pr_2
{
    // Категории товаров 
    public enum ProductCategory
    {
        Electronics = 1,
        Food,
        Clothing
    }

    // Класс Товара
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ProductCategory Category { get; set; }
        public bool IsInStock => Quantity > 0;

        public Product(int id, string name, decimal price, int quantity, ProductCategory category)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        public override string ToString()
        {
            string stockStatus = IsInStock ? $"В наличии ({Quantity} шт.)" : "НЕТ НА СКЛАДЕ";
            return $"[Код: {Id}] {Name,-18} | Категория: {Category,-11} | Цена: {Price,8:C} | Статус: {stockStatus}";
        }
    }

    // Запись о продаже
    public class SaleRecord
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime SaleTime { get; set; }

        public SaleRecord(int productId, string productName, int quantitySold, decimal totalPrice)
        {
            ProductId = productId;
            ProductName = productName;
            QuantitySold = quantitySold;
            TotalPrice = totalPrice;
            SaleTime = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{SaleTime:HH:mm:ss}] {ProductName} — {QuantitySold} шт. на сумму {TotalPrice:C}";
        }
    }

    class Program
    {
        // Генератор уникального кода 
        private static int _nextId = 1001;

        // Основной список товаров
        private static List<Product> _products = new List<Product>();

        // Стек для истории продаж
        private static Stack<SaleRecord> _salesHistory = new Stack<SaleRecord>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Заполнение списка 5 тестовыми данными
            _products.Add(new Product(_nextId++, "Ноутбук Lenovo", 55000.00m, 5, ProductCategory.Electronics));
            _products.Add(new Product(_nextId++, "Смартфон Samsung", 32000.00m, 10, ProductCategory.Electronics));
            _products.Add(new Product(_nextId++, "Яблоки (1 кг)", 120.50m, 50, ProductCategory.Food));
            _products.Add(new Product(_nextId++, "Шоколад", 90.00m, 0, ProductCategory.Food)); // Нет на складе
            _products.Add(new Product(_nextId++, "Футболка XL", 1500.00m, 15, ProductCategory.Clothing));

            bool isRunning = true;
            while (isRunning)
            {
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
                Console.Clear();

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("СПИСОК ВСЕХ ТОВАРОВ\n");
                        ShowAllProducts();
                        break;

                    case 2:
                        Console.WriteLine("ДОБАВЛЕНИЕ НОВОГО ТОВАРА\n");

                        string name = ReadNonEmptyString("Введите название товара: ");
                        decimal price = ReadDecimal("Введите цену товара (> 0): ", min: 0.01m);
                        int quantity = ReadInt("Введите количество товара на складе (>= 0): ", min: 0);

                        Console.WriteLine("\nВыберите категорию:");
                        Console.WriteLine("1. Электроника");
                        Console.WriteLine("2. Продукты питания");
                        Console.WriteLine("3. Одежда");
                        ProductCategory category = (ProductCategory)ReadInt("Категория (1-3): ", 1, 3);

                        int id = _nextId++;
                        Product newProduct = new Product(id, name, price, quantity, category);
                        _products.Add(newProduct);

                        Console.WriteLine($"\nТовар успешно добавлен! Присвоен уникальный код: {id}");
                        break;

                    case 3:
                        Console.WriteLine("УДАЛЕНИЕ ТОВАРА\n");
                        if (!ShowAllProducts()) break;

                        int idToDelete = ReadInt("\nВведите код товара для удаления: ", min: 1);
                        Product productToDelete = _products.FirstOrDefault(p => p.Id == idToDelete);

                        if (productToDelete == null)
                        {
                            Console.WriteLine("\nТовар с таким кодом не найден.");
                            break;
                        }

                        _products.Remove(productToDelete);
                        Console.WriteLine($"\nТовар \"{productToDelete.Name}\" (Код: {productToDelete.Id}) успешно удалён!");
                        break;

                    case 4:
                        Console.WriteLine("ЗАКАЗ ПОСТАВКИ ТОВАРА\n");
                        if (!ShowAllProducts()) break;

                        int idToSupply = ReadInt("\nВведите код товара для пополнения: ", min: 1);
                        Product productToSupply = _products.FirstOrDefault(p => p.Id == idToSupply);

                        if (productToSupply == null)
                        {
                            Console.WriteLine("\nТовар с таким кодом не найден.");
                            break;
                        }

                        int amountToSupply = ReadInt("Введите количество поставляемого товара (> 0): ", min: 1);
                        productToSupply.Quantity += amountToSupply;

                        Console.WriteLine($"\nПоставка оформлена. Новый остаток товара \"{productToSupply.Name}\": {productToSupply.Quantity} шт.");
                        break;

                    case 5:
                        Console.WriteLine("ПРОДАЖА ТОВАРА\n");
                        if (!ShowAllProducts()) break;

                        int idToSell = ReadInt("\nВведите код продаваемого товара: ", min: 1);
                        Product productToSell = _products.FirstOrDefault(p => p.Id == idToSell);

                        if (productToSell == null)
                        {
                            Console.WriteLine("\nТовар с таким кодом не найден.");
                            break;
                        }

                        if (!productToSell.IsInStock)
                        {
                            Console.WriteLine("\nОшибка: Данного товара нет на складе!");
                            break;
                        }

                        int amountToSell = ReadInt($"Введите количество для продажи (доступно: {productToSell.Quantity}): ", min: 1);

                        if (amountToSell > productToSell.Quantity)
                        {
                            Console.WriteLine($"\nОшибка: Недостаточно товара на складе. Доступно только {productToSell.Quantity} шт.");
                            break;
                        }

                        productToSell.Quantity -= amountToSell;
                        decimal totalSum = amountToSell * productToSell.Price;

                        _salesHistory.Push(new SaleRecord(productToSell.Id, productToSell.Name, amountToSell, totalSum));

                        Console.WriteLine($"\nПродажа совершена успешно!");
                        Console.WriteLine($"Продано: {productToSell.Name} x{amountToSell} шт.");
                        Console.WriteLine($"Сумма сделки: {totalSum:C}");
                        Console.WriteLine($"Остаток на складе: {productToSell.Quantity} шт.");
                        break;

                    case 6:
                        Console.WriteLine("ПОИСК ТОВАРОВ\n");
                        Console.WriteLine("1. Поиск по коду");
                        Console.WriteLine("2. Поиск по названию");
                        Console.WriteLine("3. Поиск по категории");
                        int mode = ReadInt("Выберите вариант поиска: ", 1, 3);

                        List<Product> results = new List<Product>();

                        switch (mode)
                        {
                            case 1:
                                int searchId = ReadInt("Введите код товара: ", min: 1);
                                results = _products.Where(p => p.Id == searchId).ToList();
                                break;
                            case 2:
                                string query = ReadNonEmptyString("Введите название или его часть: ").ToLower();
                                results = _products.Where(p => p.Name.ToLower().Contains(query)).ToList();
                                break;
                            case 3:
                                Console.WriteLine("\nВыберите категорию:");
                                Console.WriteLine("1. Электроника");
                                Console.WriteLine("2. Продукты питания");
                                Console.WriteLine("3. Одежда");
                                ProductCategory cat = (ProductCategory)ReadInt("Категория (1-3): ", 1, 3);
                                results = _products.Where(p => p.Category == cat).ToList();
                                break;
                        }

                        Console.WriteLine("\nРЕЗУЛЬТАТЫ ПОИСКА");
                        if (results.Count == 0)
                        {
                            Console.WriteLine("Товары по вашему запросу не найдены.");
                        }
                        else
                        {
                            foreach (var product in results)
                            {
                                Console.WriteLine(product);
                            }
                        }
                        break;

                    case 7:
                        Console.WriteLine("ОТМЕНА ПОСЛЕДНЕЙ ПРОДАЖИ\n");

                        if (_salesHistory.Count == 0)
                        {
                            Console.WriteLine("История продаж пуста. Нечего отменять.");
                            break;
                        }

                        SaleRecord lastSale = _salesHistory.Pop();

                        Product returnedProduct = _products.FirstOrDefault(p => p.Id == lastSale.ProductId);
                        if (returnedProduct != null)
                        {
                            returnedProduct.Quantity += lastSale.QuantitySold;
                        }

                        Console.WriteLine("ОТМЕНЕНА ПРОДАЖА:");
                        Console.WriteLine(lastSale);
                        Console.WriteLine($"Товар \"{lastSale.ProductName}\" в количестве {lastSale.QuantitySold} шт. возвращён на склад.");
                        break;

                    case 8:
                        Console.WriteLine("ОТЧЁТ О ПРОДАЖАХ\n");

                        if (_salesHistory.Count == 0)
                        {
                            Console.WriteLine("Продаж пока не совершалось.");
                            break;
                        }

                        int totalItemsSold = 0;
                        decimal totalRevenue = 0;

                        Console.WriteLine("Список всех совершённых операций:");
                        foreach (var sale in _salesHistory)
                        {
                            Console.WriteLine(sale);
                            totalItemsSold += sale.QuantitySold;
                            totalRevenue += sale.TotalPrice;
                        }

                        Console.WriteLine($"\nВсего продано товаров: {totalItemsSold} шт.");
                        Console.WriteLine($"Общая сумма выручки:  {totalRevenue:C}");
                        break;

                    case 0:
                        isRunning = false;
                        Console.WriteLine("Программа завершена. До свидания!");
                        break;
                }

                if (isRunning)
                {
                    Console.WriteLine("\nНажмите любую клавишу для возврата в меню");
                    Console.ReadKey();
                }
            }
        }

        private static bool ShowAllProducts()
        {
            if (_products.Count == 0)
            {
                Console.WriteLine("Список товаров пуст.");
                return false;
            }

            foreach (var product in _products)
            {
                Console.WriteLine(product);
            }
            return true;
        }

        private static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }
                Console.WriteLine("Ошибка: Ввод не может быть пустым!");
            }
        }

        private static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int result) && result >= min && result <= max)
                {
                    return result;
                }
                Console.WriteLine($"Ошибка: Введите целое число в диапазоне от {min} до {max}.");
            }
        }

        private static decimal ReadDecimal(string prompt, decimal min = 0)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (decimal.TryParse(input, out decimal result) && result >= min)
                {
                    return result;
                }
                Console.WriteLine($"Ошибка: Введите число не меньше {min}.");
            }
        }
    }
}
