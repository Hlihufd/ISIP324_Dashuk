using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    public class TextStatistics
    {
        public string TextPreview { get; set; }
        public int WordCount { get; set; }
        public string ShortestWord { get; set; }
        public string LongestWord { get; set; }
        public int SentenceCount { get; set; }
        public int VowelCount { get; set; }
        public int ConsonantCount { get; set; }
        public Dictionary<char, int> LetterFrequency { get; set; }
    }
    internal class Program
    {
        public static Dictionary<char, int> Statistika(string s)
        {
            Dictionary<char, int> letterCount = new Dictionary<char, int>();

            foreach (char c in s)
            {
                char carattere = Char.ToLower(c);
                if (Char.IsLetter(carattere))
                {
                    if (letterCount.ContainsKey(carattere)) letterCount[carattere]++;
                    else { letterCount[carattere] = 1; }
                }
            }
            return letterCount;
        }

        public static (string maxl, string minl) FindMinAndMaxLenght(string s)
        {
            string shortest = string.Empty;
            string longest = string.Empty;

            int minLenght = int.MaxValue;
            int maxLenght = 0;

            int wordStart = -1;

            for (int i = 0; i <= s.Length; i++)
            {
                bool isEnd = i == s.Length;
                bool isSep = !isEnd && (char.IsWhiteSpace(s[i]) || char.IsPunctuation(s[i]));

                if (!isEnd && isSep)
                {
                    if (wordStart == -1)
                    {
                        wordStart = i;
                    }
                }
                else if (wordStart != -1)
                {
                    int currentLenght = i - wordStart;

                    if (currentLenght < minLenght)
                    {
                        minLenght = currentLenght;
                        shortest = s.Substring(wordStart, currentLenght);
                    }

                    if (currentLenght > maxLenght)
                    {
                        maxLenght = currentLenght;
                        longest = s.Substring(wordStart, currentLenght);
                    }
                    wordStart = -1;
                }
            }
            return (longest, shortest);
        }

        public static int CountSent(string s)
        {
            char[] separators = new[] { '.', '!', '?', ';' };
            string[] words = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;
            foreach (var item in words)
            {
                count++;
            }
            return count;
        }

        public static int WordCount(string s)
        {
            return Regex.Matches(s, @"\w+").Count;
        }

        public static (int vowels, int consonants) CountLetters(string s)
        {
            int vCount = 0, cCount = 0;
            string sLower = s.ToLower();
            foreach (char c in sLower)
            {
                switch (c)
                {
                    case 'а':
                    case 'е':
                    case 'ё':
                    case 'и':
                    case 'о':
                    case 'у':
                    case 'ы':
                    case 'э':
                    case 'ю':
                    case 'я':
                        vCount++;
                        break;

                    case 'б':
                    case 'в':
                    case 'г':
                    case 'д':
                    case 'ж':
                    case 'з':
                    case 'й':
                    case 'к':
                    case 'л':
                    case 'щ':
                    case 'м':
                    case 'н':
                    case 'п':
                    case 'р':
                    case 'с':
                    case 'т':
                    case 'ф':
                    case 'х':
                    case 'ц':
                    case 'ч':
                    case 'ш':
                        cCount++;
                        break;
                }
            }

            return (vCount, cCount);
        }

        static void Main(string[] args)
        {
            // Список для сохранения всей статистики по прошлым текстам
            List<TextStatistics> history = new List<TextStatistics>();
            bool continueWork = true;

            while (continueWork)
            {
                Console.WriteLine("Введите текст (минимум 100 символов):");
                string input = Console.ReadLine();

                // 1. Проверка на минимальную длину
                if (string.IsNullOrEmpty(input) || input.Length < 100)
                {
                    Console.WriteLine("Ошибка: строка должна быть длиной не менее 100 символов. Попробуйте снова.");
                    continue;
                }

                // 2. Подсчёт количества слов в тексте
                int wordCount = WordCount(input);

                // 3. Поиск самого короткого и самого длинного слова
                // Используем Regex для корректного извлечения слов (игнорируя знаки препинания и пробелы)
                var words = Regex.Matches(input, @"\p{L}+").Cast<Match>().Select(m => m.Value).ToList();
                string shortestWord = words.Any() ? words.OrderBy(w => w.Length).ThenBy(w => w).First() : string.Empty;
                string longestWord = words.Any() ? words.OrderByDescending(w => w.Length).ThenBy(w => w).First() : string.Empty;

                // 4. Подсчёт количества предложений
                int sentenceCount = CountSent(input);

                // 5. Подсчёт количества гласных и согласных букв
                var letterCounts = CountLetters(input);

                // 6. Создание статистики по частоте встречаемости каждой буквы
                Dictionary<char, int> letterFreq = Statistika(input);

                // 7. Сохранение всей статистики в список
                TextStatistics stats = new TextStatistics
                {
                    TextPreview = input.Length > 40 ? input.Substring(0, 40) + "..." : input,
                    WordCount = wordCount,
                    ShortestWord = shortestWord,
                    LongestWord = longestWord,
                    SentenceCount = sentenceCount,
                    VowelCount = letterCounts.vowels,
                    ConsonantCount = letterCounts.consonants,
                    LetterFrequency = letterFreq
                };

                history.Add(stats);

                // Вывод статистики текущего текста
                Console.WriteLine("\n--- Статистика текущего текста ---");
                Console.WriteLine($"Количество слов: {stats.WordCount}");
                Console.WriteLine($"Самое короткое слово: \"{stats.ShortestWord}\"");
                Console.WriteLine($"Самое длинное слово: \"{stats.LongestWord}\"");
                Console.WriteLine($"Количество предложений: {stats.SentenceCount}");
                Console.WriteLine($"Гласных букв: {stats.VowelCount}");
                Console.WriteLine($"Согласных букв: {stats.ConsonantCount}");

                Console.WriteLine("Частота встречаемости букв:");
                foreach (var kvp in stats.LetterFrequency.OrderByDescending(kvp => kvp.Value).ThenBy(kvp => kvp.Key))
                {
                    Console.WriteLine($"  '{kvp.Key}': {kvp.Value}");
                }

                // 8. Меню действий (продолжить, история, выход)
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Продолжить работу с новым текстом");
                Console.WriteLine("2. Показать статистику по прошлым текстам");
                Console.WriteLine("3. Выход");

                string choice = Console.ReadLine();

                if (choice == "2")
                {
                    Console.WriteLine("\n--- История статистики ---");
                    if (history.Count == 0)
                    {
                        Console.WriteLine("История пуста.");
                    }
                    else
                    {
                        for (int i = 0; i < history.Count; i++)
                        {
                            var h = history[i];
                            Console.WriteLine($"\nТекст #{i + 1} (Начало: \"{h.TextPreview}\")");
                            Console.WriteLine($"Слов: {h.WordCount}, Предложений: {h.SentenceCount}");
                            Console.WriteLine($"Краткое: '{h.ShortestWord}', Длинное: '{h.LongestWord}'");
                            Console.WriteLine($"Гласных: {h.VowelCount}, Согласных: {h.ConsonantCount}");
                        }
                    }
                    Console.WriteLine("\nНажмите Enter для продолжения...");
                    Console.ReadLine();
                }
                else if (choice == "3")
                {
                    continueWork = false;
                }
            }
        }
    }
}
