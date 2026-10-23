using System;

namespace ISIP324_Dashuk
{
    internal static class ConsoleHelper
    {
        // Читает число, пока пользователь не введёт правильно
        public static int ReadInt(string text)
        {
            while (true)
            {
                Console.Write(text);
                if (int.TryParse(Console.ReadLine(), out int number)) return number;
                Console.WriteLine("Введите число");
            }
        }

        // Читает непустую строку без пробелов по краям и не длиннее maxLength
        public static string ReadText(string text, int maxLength)
        {
            while (true)
            {
                Console.Write(text);
                string value = (Console.ReadLine() ?? "").Trim();
                if (value.Length == 0) Console.WriteLine("Поле не может быть пустым");
                else if (value.Length > maxLength) Console.WriteLine($"Слишком длинно (максимум {maxLength} символов)");
                else return value;
            }
        }
    }
}