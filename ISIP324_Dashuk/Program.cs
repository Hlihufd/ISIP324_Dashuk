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

    internal class Program
    {
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
                else if (wordStart != -1) {
                    int currentLenght = i - wordStart;

                    if (currentLenght < minLenght) { 
                        minLenght = currentLenght;
                        shortest = s.Substring(wordStart, currentLenght);
                    }

                    if (currentLenght > maxLenght) {
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

        public static int WordCount(string s) {
            return Regex.Matches(s, @"\w+").Count;
        }

        public static (int vowels, int consonants) CountLetters(string s)
        {
            int vCount = 0, cCount = 0;

            foreach (char c in s)
            {
                switch (c)
                {
                    case 'а':
                    case 'е':
                    case 'ё':
                    case 'и':
                    case 'о':
                    case 'Ё':
                    case 'Я':
                    case 'у':
                    case 'ы':
                    case 'э':
                    case 'ю':
                    case 'я':
                    case 'А':
                    case 'Е':
                    case 'И':
                    case 'О':
                    case 'У':
                    case 'Ы':
                    case 'Э':
                    case 'Ю':
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
                    case 'Н':
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
                    case 'Б':
                    case 'В':
                    case 'Г':
                    case 'Д':
                    case 'Ж':
                    case 'З':
                    case 'Й':
                    case 'К':
                    case 'Л':
                    case 'М':
                    case 'П':
                    case 'Р':
                    case 'С':
                    case 'Т':
                    case 'Ф':
                    case 'Х':
                    case 'Ц':
                    case 'Ч':
                    case 'Ш':
                    case 'Щ':
                        cCount++;
                        break;
                }
            }

            return (vCount, cCount);
        }

        static void Main(string[] args)
        {
            Console.WriteLine(CountLetters("вавававааа"));
            Console.WriteLine(WordCount(" Привет,   как   дела? "));
            Console.WriteLine(CountSent("sыввыввыв ыв ы ыв вввы. ывывывыв ывафаа уа фва! ывывуап ва ууу аа ? ывывау  ауаава уау;"));
        }
    }
}
