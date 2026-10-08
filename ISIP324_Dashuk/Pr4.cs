using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp4
{
    public enum Genre
    {
        Roman = 1,
        Novel,
        Adventure,
        Fantasy,
        Horror
    }

    public class Book
    {
        private static int _nextId = 1;
        public int ID { get; }
        public string Title;
        public string Author;
        public Genre Genre;
        public int Year;
        public decimal Price;

        public Book(string title, string author, Genre genre, int year, decimal price)
        {
            ID = _nextId++;
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public void PrintRow()
        {
            string titleDisplay = Title.Length > 25 ? Title.Substring(0, 22) + "..." : Title;
            string authorDisplay = Author.Length > 20 ? Author.Substring(0, 17) + "..." : Author;

            Console.WriteLine($"| {ID,-3} | {titleDisplay,-25} | {authorDisplay,-20} | {Genre,-10} | {Year,-4} | {Price,-8:F2} |");
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            List<Book> books = new List<Book>
            {
                new Book("Война и мир", "Лев Толстой", Genre.Roman, 1869, 999.90m),
                new Book("1984", "Джордж Оруэлл", Genre.Novel, 1949, 750.50m),
                new Book("Остров сокровищ", "Роберт Льюис Стивенсон", Genre.Adventure, 1883, 620.00m),
                new Book("Гарри Поттер и философский камень", "Дж. К. Роулинг", Genre.Fantasy, 1997, 890.30m),
                new Book("Оно", "Стивен Кинг", Genre.Horror, 1986, 850.75m)
            };

            int n = -1;
            while (n != 0)
            {
                Console.WriteLine("\nМЕНЮ");
                Console.WriteLine("1 — Показать все книги");
                Console.WriteLine("2 — Удалить книгу по ID");
                Console.WriteLine("3 — Сортировать книги");
                Console.WriteLine("4 — Мин/макс цена");
                Console.WriteLine("5 — Количество книг по авторам");
                Console.WriteLine("6 — Добавить новую книгу");
                Console.WriteLine("7 — Поиск книг");
                Console.WriteLine("0 — Выход");
                Console.Write("Выберите действие: ");

                if (!int.TryParse(Console.ReadLine(), out n))
                {
                    Console.WriteLine("Ошибка: введите число.");
                    continue;
                }

                switch (n)
                {
                    case 1:
                        if (!books.Any())
                        {
                            Console.WriteLine("Список книг пуст.");
                            break;
                        }

                        Console.WriteLine(new string('-', 95));
                        Console.WriteLine($"| {"ID",-3} | {"Название",-25} | {"Автор",-20} | {"Жанр",-10} | {"Год",-4} | {"Цена",-8} |");
                        Console.WriteLine(new string('-', 95));

                        foreach (Book book in books)
                        {
                            book.PrintRow();
                        }

                        Console.WriteLine(new string('-', 95));
                        break;

                    case 2:
                        Console.Write("Введите ID книги для удаления: ");
                        Book removed = null;
                        if (int.TryParse(Console.ReadLine(), out int id) && (removed = books.FirstOrDefault(b => b.ID == id)) != null)
                        {
                            books.Remove(removed);
                            Console.WriteLine($"Книга \"{removed.Title}\" (ID: {removed.ID}) успешно удалена.");
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка: неверный ID или список пуст. Проверьте диапазон (1–{books.Count}).");
                        }
                        break;

                    case 3:
                        Console.WriteLine("Выберите способ сортировки:");
                        Console.WriteLine("1 — По названию (А–Я)");
                        Console.WriteLine("2 — По году издания");

                        if (!int.TryParse(Console.ReadLine(), out int n1))
                        {
                            Console.WriteLine("Неверный ввод.");
                            break;
                        }

                        switch (n1)
                        {
                            case 1:
                                books = books.OrderBy(b => b.Title, StringComparer.Ordinal).ToList();
                                Console.WriteLine("Отсортировано по названию.");
                                break;
                            case 2:
                                books = books.OrderBy(b => b.Year).ToList();
                                Console.WriteLine("Отсортировано по году.");
                                break;
                            default:
                                Console.WriteLine("Неверная опция.");
                                break;
                        }
                        break;

                    case 4:
                        if (books.Any())
                        {
                            Console.WriteLine("Статистика цен:");
                            Console.WriteLine($"Минимальная цена: {books.Min(b => b.Price):F2}");
                            Console.WriteLine($"Максимальная цена: {books.Max(b => b.Price):F2}");
                        }
                        else
                        {
                            Console.WriteLine("Список книг пуст, нечего анализировать.");
                        }
                        break;

                    case 5:
                        Console.WriteLine("Количество книг по авторам:");
                        Dictionary<string, int> dictionary = books.GroupBy(p => p.Author).ToDictionary(p => p.Key, p => p.Count());

                        if (!dictionary.Any())
                        {
                            Console.WriteLine("   Нет данных.");
                        }
                        else
                        {
                            foreach (KeyValuePair<string, int> item in dictionary)
                            {
                                Console.WriteLine($"   {item.Key}: {item.Value} шт.");
                            }
                        }
                        break;

                    case 6:
                        string title;
                        while (true)
                        {
                            Console.Write("Название книги: ");
                            title = Console.ReadLine()?.Trim();
                            if (!string.IsNullOrEmpty(title))
                            {
                                break;
                            }

                            Console.WriteLine("Ошибка: название не может быть пустым.");
                        }

                        string author;
                        while (true)
                        {
                            Console.Write("Автор: ");
                            author = Console.ReadLine()?.Trim();
                            if (!string.IsNullOrEmpty(author))
                            {
                                break;
                            }

                            Console.WriteLine("Ошибка: автор не может быть пустым.");
                        }

                        Genre genre;
                        while (true)
                        {
                            Console.Write($"Жанр (выберите один: {string.Join(", ", Enum.GetValues(typeof(Genre)).Cast<Genre>())}): ");
                            string input = Console.ReadLine()?.Trim();

                            if (Enum.TryParse<Genre>(input, true, out genre))
                            {
                                break;
                            }

                            Console.WriteLine("Ошибка: неверный жанр.");
                        }

                        int year;
                        int currentYear = DateTime.Now.Year;
                        while (true)
                        {
                            Console.Write($"Год издания (от 1000 до {currentYear}): ");
                            string input = Console.ReadLine()?.Trim();

                            if (int.TryParse(input, out year) && year >= 1000 && year <= currentYear)
                            {
                                break;
                            }

                            Console.WriteLine($"Ошибка: год должен быть числом от 1000 до {currentYear}.");
                        }

                        decimal price;
                        while (true)
                        {
                            Console.Write("Цена (должна быть больше 0): ");
                            string input = Console.ReadLine()?.Trim();

                            if (decimal.TryParse(input, out price) && price > 0)
                            {
                                break;
                            }

                            Console.WriteLine("Ошибка: цена должна быть положительным числом.");
                        }

                        Book newBook = new Book(title, author, genre, year, price);
                        books.Add(newBook);
                        Console.WriteLine($"\nКнига успешно добавлена!");
                        Console.WriteLine($"ID: {newBook.ID}");
                        Console.WriteLine($"\"{newBook.Title}\"");
                        Console.WriteLine($"{newBook.Author}");
                        Console.WriteLine($"Цена: {newBook.Price:F2}");
                        break;

                    case 7:
                        Console.WriteLine("Критерий поиска:");
                        Console.WriteLine("1 — по названию");
                        Console.WriteLine("2 — по автору");
                        Console.WriteLine("3 — по жанру");

                        string option = Console.ReadLine()?.Trim() ?? "";
                        List<Book> found = new List<Book>();

                        if (option == "1")
                        {
                            Console.Write("Часть названия: ");
                            string text = Console.ReadLine()?.Trim() ?? "";
                            found = books.Where(b => b.Title.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                        }
                        else if (option == "2")
                        {
                            Console.Write("Часть имени автора: ");
                            string text = Console.ReadLine()?.Trim() ?? "";
                            found = books.Where(b => b.Author.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                        }
                        else if (option == "3")
                        {
                            Console.Write($"Жанр ({string.Join(", ", Enum.GetValues(typeof(Genre)).Cast<Genre>())}): ");
                            string input = Console.ReadLine()?.Trim() ?? "";

                            if (Enum.TryParse<Genre>(input, true, out Genre g))
                            {
                                found = books.Where(b => b.Genre == g).ToList();
                            }
                            else
                            {
                                Console.WriteLine("Неверный жанр.");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Выберите 1, 2 или 3.");
                            break;
                        }

                        if (!found.Any())
                        {
                            Console.WriteLine("\nНичего не найдено по вашему запросу.");
                        }
                        else
                        {
                            Console.WriteLine($"\nНайдено книг: {found.Count}");
                            Console.WriteLine(new string('-', 95));
                            Console.WriteLine($"| {"ID",-3} | {"Название",-25} | {"Автор",-20} | {"Жанр",-10} | {"Год",-4} | {"Цена",-8} |");
                            Console.WriteLine(new string('-', 95));

                            foreach (Book b in found)
                            {
                                b.PrintRow();
                            }

                            Console.WriteLine(new string('-', 95));
                        }
                        break;

                    case 0:
                        break;

                    default:
                        Console.WriteLine("Неверная опция. Выберите от 0 до 7.");
                        break;
                }
            }
        }
    }
}
