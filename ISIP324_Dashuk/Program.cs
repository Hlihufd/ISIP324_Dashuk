using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ISIP324_Dashuk
{
    internal class Program
    {
        static void Bubble (int[] array)
        {
            int n = array.Length;
            bool swapped;

            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    break; 
                }
            }
        }
        static void Main(string[] args)
        {
            int n = 1;
            Console.WriteLine("vvedi kolichestvo iteraciy");
            int count = Convert.ToInt32(Console.ReadLine());
            int[] nums = new int[count];
            List<string> pokupki = new List<string>();
            for (int i = 1; i <= count; ++i) {
                Console.WriteLine($"покупка {i}");
                pokupki.Add(Console.ReadLine());
            }

            while (n != 0) {
                Console.WriteLine("viberi");
                n = Convert.ToInt32(Console.ReadLine());
                switch (n)
                {
                    case 1:
                        foreach( string s in pokupki){ 
                            Console.WriteLine(s);
                        }
                        break;
                    case 2:
                        int schet = 0;
                        foreach (string s in pokupki) {
                            string[] words = s.Split(new char[] { ';' });
                            nums[schet] = Convert.ToInt32(words[1]);
                            schet++;
                        }
                        int summa = 0;
                        foreach (int i in nums) {
                            summa += i;
                        }
                        Bubble(nums);
                        Console.WriteLine($"max: {nums[nums.Length-1]}, min: {nums[0]}, middle: {summa / nums.Length}");
                        break;
                    case 3:
                        Bubble(nums);
                        break;
                    case 4:
                        
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
