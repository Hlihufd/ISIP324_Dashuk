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

            int minLength = int.MaxValue;
            int maxLength = 0;

            int wordStart = -1;

            for (int i = 0; i <= s.Length; i++)
            {
                bool isEnd = i == s.Length;
                bool isSep = !isEnd && (char.IsWhiteSpace(s[i]) || char.IsPunctuation(s[i]));

                if (!isSep && wordStart == -1)
                {
                    wordStart = i;
                }
                else if (isSep && wordStart != -1)
                {
                    int currentLength = i - wordStart;

                    if (currentLength < minLength)
                    {
                        minLength = currentLength;
                        shortest = s.Substring(wordStart, currentLength);
                    }

                    if (currentLength > maxLength)
                    {
                        maxLength = currentLength;
                        longest = s.Substring(wordStart, currentLength);
                    }

                    wordStart = -1; 
                }
            }

            if (minLength == int.MaxValue)
            {
                shortest = string.Empty;
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
            List<TextStatistics> history = new List<TextStatistics>();
            bool continueWork = true;

            while (continueWork)
            {
                Console.WriteLine("Введите текст (минимум 100 символов):");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input) || input.Length < 100)
                {
                    Console.WriteLine("Ошибка: строка должна быть длиной не менее 100 символов. Попробуйте снова.");
                    continue;
                }

                int wordCount = WordCount(input);

                var (maxl, minl) = FindMinAndMaxLenght(input);

                int sentenceCount = CountSent(input);

                var letterCounts = CountLetters(input);

                Dictionary<char, int> letterFreq = Statistika(input);

                TextStatistics stats = new TextStatistics
                {
                    TextPreview = input.Length > 40 ? input.Substring(0, 40) + "..." : input,
                    WordCount = wordCount,
                    ShortestWord = minl,
                    LongestWord = maxl,
                    SentenceCount = sentenceCount,
                    VowelCount = letterCounts.vowels,
                    ConsonantCount = letterCounts.consonants,
                    LetterFrequency = letterFreq
                };

                history.Add(stats);

                Console.WriteLine("\nСтатистика текущего текста");
                Console.WriteLine($"Количество слов: {stats.WordCount}");
                Console.WriteLine($"Самое короткое слово: \"{stats.ShortestWord}\"");
                Console.WriteLine($"Самое длинное слово: \"{stats.LongestWord}\"");
                Console.WriteLine($"Количество предложений: {stats.SentenceCount}");
                Console.WriteLine($"Гласных букв: {stats.VowelCount}");
                Console.WriteLine($"Согласных букв: {stats.ConsonantCount}");

                Console.WriteLine("Частота встречаемости букв:");
                var pairs = new (char Key, int Value)[stats.LetterFrequency.Count];
                int j = 0;
                foreach (var kvp in stats.LetterFrequency)
                    pairs[j++] = (kvp.Key, kvp.Value);

                foreach (var (key, value) in pairs)
                    Console.Write($"'{key}': {value},  ");

                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Продолжить работу с новым текстом");
                Console.WriteLine("2. Показать статистику по прошлым текстам");
                Console.WriteLine("3. Выход");

                string choice = Console.ReadLine();

                if (choice == "2")
                {
                    Console.WriteLine("\nИстория статистики");
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
                            Console.WriteLine("Частота встречаемости букв:");
                            int j1 = 0;
                            var pairs1 = new (char Key, int Value)[stats.LetterFrequency.Count];
                            foreach (var kvp1 in stats.LetterFrequency)
                                pairs1[j1++] = (kvp1.Key, kvp1.Value);

                            foreach (var (key, value) in pairs)
                                Console.Write($"'{key}': {value},  ");
                        }
                    }
                    Console.WriteLine("\nНажмите Alt+F4 для продолжения...");
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
