using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ISIP324_Dashuk
{
    public static class ConsolHelper
    {
        public static void Title(string text)
        {
            Console.WriteLine();
            Console.WriteLine("   " + text);
        }

        private static void Print(string text, ConsoleColor color)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }

        public static void Success(string text)
        {
            Print(text, ConsoleColor.Green);
        }

        public static void Error(string text) {
            Print(text, ConsoleColor.Red);
        }

        public static void Info(string text) {
            Print(text, ConsoleColor.Yellow);
        }

        // чтение непустой строки
        public static string ReadString(string text) {
            while (true)
            {
                Console.Write(text);
                string value = (Console.ReadLine() ?? "").Trim();
                if (value.Length > 0) return value;
                Error("Поле не может быть пустым");
            }
        }

        public static int ReadInt(string text) {
            while (true) {
                Console.Write(text);
                int value;
                if(int.TryParse(Console.ReadLine(), out value)) return value;
                Error("Введите целое число");
            }
        }

        public static int ReadPositiveInt(string text) {
            while (true) {
                int value = ReadInt(text);
                if(value > 0) return value;
                Error("Число должно быть больше нуля");
            }
        }

        // ввод пароля,звезды
        public static string ReadPassword(string text) {
            Console.Write(text);
            var sb = new StringBuilder();
            while (true) { 
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) {
                    Console.WriteLine();
                    return sb.ToString();
                }
                if (key.Key == ConsoleKey.Backspace) {
                    if (sb.Length > 0) { 
                        sb.Length--;
                        Console.Write("\b \b");
                    }
                }else if (!char.IsControl(key.KeyChar)) { 
                    sb.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
        }
    }
}