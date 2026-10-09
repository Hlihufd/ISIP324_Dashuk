using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp4
{
    public class People
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        private string _phone_number;

        public string PhoneNumber
        {
            get => _phone_number;
            set
            {
                if (!IsValidPhoneNumber(value))
                    throw new ArgumentException(
                        "Некорректный номер телефона: должен содержать только цифры и иметь длину 10–15 символов.");
                _phone_number = value;
            }
        }

        public People(string name, int age, string phoneNumber, string address)
        {
            Name = name;
            Age = age;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        // Возвращаем private, как вы и просили
        private static bool IsValidPhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return false;

            foreach (var c in number)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return number.Length >= 10 && number.Length <= 15;
        }

    }

    public enum Kurs
    {
        First = 1,
        Second,
        Third,
        Fourth
    }

    public enum Subject
    {
        Maths = 1,
        English,
        Physics,
        Computer_Science,
        History
    }

    public class Student : People
    {
        public Kurs Course { get; }

        public List<Subject> Subjects { get; } = new List<Subject>();

        public Student(string name, int age, string phoneNumber, string address, Kurs course, Subject subject)
            : base(name, age, phoneNumber, address)
        {
            Course = course;
            Subjects.Add(subject);
        }
    }

    public class Teacher : People
    {
        public Subject Subject { get; }
        public List<Student> Students { get; }

        public Teacher(string name, int age, string phoneNumber, string address, Subject subject, List<Student> students)
            : base(name, age, phoneNumber, address)
        {
            Subject = subject;
            Students = students ?? new List<Student>();
        }
    }

    internal class Program
    {
        private static List<Student> allStudents = new List<Student>();
        private static List<Teacher> allTeachers = new List<Teacher>();
        private static bool TryValidatePhone(string phone)
        {
            try
            {
                var temp = new People("", 0, phone, "");
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static void AddStudent()
        {
            Console.WriteLine("\nДобавление студента");
            Console.Write("Введите имя студента: ");
            string name = Console.ReadLine();

            Console.Write("Введите возраст студента: ");
            int age = 0;
            while (!int.TryParse(Console.ReadLine(), out age) || age <= 0 || age > 120)
            {
                Console.Write("Некорректный возраст. Введите целое число от 1 до 120: ");
            }

            Console.Write("Введите номер телефона студента: ");
            string phoneNumber = Console.ReadLine();
            while (!TryValidatePhone(phoneNumber))
            {
                Console.Write("Некорректный номер. Введите только цифры (10-15 знаков): ");
                phoneNumber = Console.ReadLine();
            }

            Console.Write("Введите адрес студента: ");
            string address = Console.ReadLine();

            Console.Write($"Введите курс студента (1-{Enum.GetNames(typeof(Kurs)).Length}): ");
            int courseID;
            while (!int.TryParse(Console.ReadLine(), out courseID) || !Enum.IsDefined(typeof(Kurs), courseID))
            {
                Console.Write($"Некорректный курс. Введите число от 1 до {Enum.GetNames(typeof(Kurs)).Length}: ");
            }
            Kurs course = (Kurs)courseID;

            Console.Write($"Введите предмет от 1 до {Enum.GetNames(typeof(Subject)).Length}: ");
            int subID;
            while (!int.TryParse(Console.ReadLine(), out subID) || 
                !Enum.IsDefined(typeof(Subject), subID))
            {
                Console.Write($"Некорректный предмет. Введите число от 1 до {Enum.GetNames(typeof(Subject)).Length}: ");
            }
            Subject sub = (Subject)subID;

            try
            {
                Student student = new Student(name, age, phoneNumber, address, course, sub);

                allStudents.Add(student);

                Console.WriteLine("\nСтудент успешно создан и добавлен в базу!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании: {ex.Message}");
            }
        }
        private static void AddTeacher()
        {
            Console.WriteLine("\nДобавление преподавателя");
            Console.WriteLine("Введите имя преподавателя:");
            string name = Console.ReadLine();

            Console.Write("Введите возраст преподавателя: ");
            int age = 0;
            while (!int.TryParse(Console.ReadLine(), out age) || age <= 20 || age > 120)
            {
                Console.Write("Некорректный возраст. Введите целое число от 21 до 120: ");
            }

            Console.Write("Введите номер телефона преподавателя: ");
            string phoneNumber = Console.ReadLine();
            while (!TryValidatePhone(phoneNumber))
            {
                Console.Write("Некорректный номер. Введите только цифры (10-15 знаков): ");
                phoneNumber = Console.ReadLine();
            }

            Console.Write("Введите адрес преподавателя: ");
            string address = Console.ReadLine();

            Console.WriteLine($"Введите предмет преподавателя от 1 до {Enum.GetNames(typeof(Subject)).Length}: ");
            int subID;
            while (!int.TryParse(Console.ReadLine(), out subID) || !Enum.IsDefined(typeof(Subject), subID))
            {
                Console.Write($"Некорректный предмет. Введите число от 1 до {Enum.GetNames(typeof(Subject)).Length}: ");
            }
            Subject teachersub = (Subject)subID;

            List<Student> assignedStudents = allStudents.Where(p => p.Subjects.Contains(teachersub)).ToList();

            try
            {
                Teacher teacher = new Teacher(name, age, phoneNumber, address, teachersub, assignedStudents);
                allTeachers.Add(teacher);

                Console.WriteLine($"\nПреподаватель успешно создан!");
                Console.WriteLine($"К нему автоматически привязано {assignedStudents.Count} студентов по предмету '{teachersub}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании {ex.Message}"); ;
            }
        }
        static void ShowMenu()
        {
            Console.WriteLine("\nMenu");
            Console.WriteLine("1 – Добавить студента");
            Console.WriteLine("2 – Добавить преподавателя");
            Console.WriteLine("3 – Показать всех преподавателей и их студентов");
            Console.WriteLine("0 – Выход");
            Console.Write("Выберите действие:");
        }
        static void Main(string[] args)
        {

            int n;

           
            while (true)
            {
                ShowMenu();

                if (!int.TryParse(Console.ReadLine(), out n))
                {
                    Console.WriteLine("Ошибка: введите корректное число.");
                    continue; 
                }

                switch (n)
                {
                    case 1:
                        AddStudent();
                        break;
                    case 2:
                        AddTeacher();
                        break;
                    case 3:
                        foreach(var teacher in allTeachers)
                        {
                            Console.WriteLine($"{teacher.Name}, {teacher.Age}, {teacher.PhoneNumber}, {teacher.Subject}, студенты: ");
                            foreach (var student in teacher.Students) Console.WriteLine(student.Name);
                        }
                        break;
                    case 0:
                        Console.WriteLine("Выход из программы.");
                        return; 
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

    }
}
