using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp4
{
    // АБСТРАКЦИЯ + НАСЛЕДОВАНИЕ 
    // Общая абстрактная база для студентов и преподавателей
    public abstract class Person
    {
        private string _name;
        private int _age;
        private string _address;
        private string _phoneNumber;

        protected Person(string name, int age, string phoneNumber, string address)
        {
            Name = name;
            Age = age;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя не может быть пустым.");
                _name = value.Trim();
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                if (value <= 0 || value > 120)
                    throw new ArgumentException("Возраст должен быть от 1 до 120.");
                _age = value;
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Адрес не может быть пустым.");
                _address = value.Trim();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (!IsValidPhoneNumber(value))
                    throw new ArgumentException(
                        "Некорректный номер телефона: только цифры, длина 10–15 символов.");
                _phoneNumber = value;
            }
        }

        public static bool IsValidPhoneNumber(string number)
        {
            return !string.IsNullOrWhiteSpace(number)
                   && number.All(char.IsDigit)
                   && number.Length >= 10 && number.Length <= 15;
        }

        // Абстрактное свойство: каждый наследник сам говорит, кто он
        public abstract string Role { get; }

        // ПОЛИМОРФИЗМ 
        // Виртуальный метод: наследники расширяют вывод информации
        public virtual string GetInfo()
        {
            return $"[{Role}] {Name}, {Age} лет, тел.: {PhoneNumber}, адрес: {Address}";
        }

        public override string ToString() => $"{Name} ({Role})";
    }

    public enum Year
    {
        First = 1,
        Second,
        Third,
        Fourth
    }

    public class Student : Person
    {
        private readonly List<Course> _courses = new List<Course>();

        public Student(string name, int age, string phoneNumber, string address, Year year)
            : base(name, age, phoneNumber, address)
        {
            Year = year;
        }

        public Year Year { get; }

        // Наружу отдаём только чтение — список менять напрямую нельзя
        public IReadOnlyList<Course> Courses => _courses;

        public override string Role => "Студент";

        public override string GetInfo()
        {
            string courses = _courses.Count == 0
                ? "нет"
                : string.Join(", ", _courses.Select(c => c.Title));
            return base.GetInfo() + $", курс обучения: {(int)Year}, записан на: {courses}";
        }

        // internal: вызывается только из Course.Enroll, чтобы связь была двусторонней
        internal void AddCourse(Course course) => _courses.Add(course);
    }

    public class Teacher : Person
    {
        private readonly List<Course> _courses = new List<Course>();

        public Teacher(string name, int age, string phoneNumber, string address)
            : base(name, age, phoneNumber, address)
        {
        }

        public IReadOnlyList<Course> Courses => _courses;

        public override string Role => "Преподаватель";

        public override string GetInfo()
        {
            string courses = _courses.Count == 0
                ? "нет"
                : string.Join(", ", _courses.Select(c => c.Title));
            return base.GetInfo() + $", ведёт курсы: {courses}";
        }

        internal void AddCourse(Course course) => _courses.Add(course);
        internal void RemoveCourse(Course course) => _courses.Remove(course);
    }

    public class Course
    {
        private readonly List<Student> _students = new List<Student>();
        private string _title;

        public Course(int id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description ?? "";
        }

        public int Id { get; }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название курса не может быть пустым.");
                _title = value.Trim();
            }
        }

        public string Description { get; set; }
        public Teacher Teacher { get; private set; }
        public IReadOnlyList<Student> Students => _students;

        // Запись студента: обновляет обе стороны связи
        public bool Enroll(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (_students.Contains(student)) return false;

            _students.Add(student);
            student.AddCourse(this);
            return true;
        }

        // Назначение преподавателя (старого снимаем с курса)
        public void AssignTeacher(Teacher teacher)
        {
            if (teacher == null) throw new ArgumentNullException(nameof(teacher));
            if (Teacher == teacher) return;

            Teacher?.RemoveCourse(this);
            Teacher = teacher;
            teacher.AddCourse(this);
        }

        public string GetInfo()
        {
            string teacher = Teacher == null ? "не назначен" : Teacher.Name;
            return $"Курс #{Id}: {Title}\n  Описание: {Description}\n  Преподаватель: {teacher}\n  Студентов записано: {_students.Count}";
        }

        public override string ToString() => $"#{Id} {Title}";
    }

    // БАЗА УНИВЕРСИТЕТА 
    public class University
    {
        private readonly List<Student> _students = new List<Student>();
        private readonly List<Teacher> _teachers = new List<Teacher>();
        private readonly List<Course> _courses = new List<Course>();
        private int _nextCourseId = 1;

        public IReadOnlyList<Student> Students => _students;
        public IReadOnlyList<Teacher> Teachers => _teachers;
        public IReadOnlyList<Course> Courses => _courses;

        public void AddStudent(Student s) => _students.Add(s);
        public void AddTeacher(Teacher t) => _teachers.Add(t);

        public Course CreateCourse(string title, string description)
        {
            var course = new Course(_nextCourseId++, title, description);
            _courses.Add(course);
            return course;
        }
    }

    internal class Program
    {
        private static readonly University University = new University();

        // ВСПОМОГАТЕЛЬНЫЙ ВВОД 
        private static string ReadNonEmpty(string prompt)
        {
            Console.Write(prompt);
            string s = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(s))
            {
                Console.Write("Поле не может быть пустым. Повторите: ");
                s = Console.ReadLine();
            }
            return s.Trim();
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            Console.Write(prompt);
            int value;
            while (!int.TryParse(Console.ReadLine(), out value) || value < min || value > max)
            {
                Console.Write($"Введите целое число от {min} до {max}: ");
            }
            return value;
        }

        private static string ReadPhone(string prompt)
        {
            Console.Write(prompt);
            string phone = Console.ReadLine();
            while (!Person.IsValidPhoneNumber(phone))
            {
                Console.Write("Некорректный номер. Только цифры (10-15 знаков): ");
                phone = Console.ReadLine();
            }
            return phone;
        }

        // Выбор элемента из списка по номеру; null, если список пуст
        private static T Select<T>(string title, IReadOnlyList<T> items) where T : class
        {
            if (items.Count == 0)
            {
                Console.WriteLine("Список пуст.");
                return null;
            }

            Console.WriteLine(title);
            for (int i = 0; i < items.Count; i++)
                Console.WriteLine($"  {i + 1}. {items[i]}");

            int index = ReadInt("Ваш выбор: ", 1, items.Count);
            return items[index - 1];
        }

        // ДЕЙСТВИЯ МЕНЮ 
        private static void AddStudent()
        {
            Console.WriteLine("\nДобавление студента");
            string name = ReadNonEmpty("Имя: ");
            int age = ReadInt("Возраст (1-120): ", 1, 120);
            string phone = ReadPhone("Телефон: ");
            string address = ReadNonEmpty("Адрес: ");
            int year = ReadInt("Курс обучения (1-4): ", 1, 4);

            University.AddStudent(new Student(name, age, phone, address, (Year)year));
            Console.WriteLine("Студент добавлен.");
        }

        private static void AddTeacher()
        {
            Console.WriteLine("\nДобавление преподавателя");
            string name = ReadNonEmpty("Имя: ");
            int age = ReadInt("Возраст (21-120): ", 21, 120);
            string phone = ReadPhone("Телефон: ");
            string address = ReadNonEmpty("Адрес: ");

            University.AddTeacher(new Teacher(name, age, phone, address));
            Console.WriteLine("Преподаватель добавлен.");
        }

        private static void CreateCourse()
        {
            Console.WriteLine("\nСоздание курса");
            string title = ReadNonEmpty("Название курса: ");
            Console.Write("Описание: ");
            string description = Console.ReadLine();

            Course course = University.CreateCourse(title, description);
            Console.WriteLine($"Курс создан: {course}");

            if (University.Teachers.Count > 0)
            {
                Console.Write("Назначить преподавателя сейчас? (y/n): ");
                if ((Console.ReadLine() ?? "").Trim().ToLower() == "y")
                {
                    Teacher t = Select("Выберите преподавателя:", University.Teachers);
                    if (t != null)
                    {
                        course.AssignTeacher(t);
                        Console.WriteLine($"Преподаватель {t.Name} назначен.");
                    }
                }
            }
        }

        private static void EnrollStudent()
        {
            Console.WriteLine("\nЗапись студента на курс");
            Student student = Select("Выберите студента:", University.Students);
            if (student == null) return;

            Course course = Select("Выберите курс:", University.Courses);
            if (course == null) return;

            if (course.Enroll(student))
                Console.WriteLine($"{student.Name} записан(а) на курс «{course.Title}».");
            else
                Console.WriteLine("Студент уже записан на этот курс.");
        }

        private static void AssignTeacher()
        {
            Console.WriteLine("\nНазначение преподавателя на курс");
            Teacher teacher = Select("Выберите преподавателя:", University.Teachers);
            if (teacher == null) return;

            Course course = Select("Выберите курс:", University.Courses);
            if (course == null) return;

            course.AssignTeacher(teacher);
            Console.WriteLine($"{teacher.Name} теперь ведёт курс «{course.Title}».");
        }

        private static void ShowStudentCourses()
        {
            Student student = Select("\nВыберите студента:", University.Students);
            if (student == null) return;

            Console.WriteLine($"Курсы студента {student.Name}:");
            if (student.Courses.Count == 0)
                Console.WriteLine("  (не записан ни на один курс)");
            foreach (var c in student.Courses)
                Console.WriteLine($"  - {c}");
        }

        private static void ShowStudentInfo()
        {
            Student student = Select("\nВыберите студента:", University.Students);
            if (student != null) Console.WriteLine(student.GetInfo());
        }

        private static void ShowTeacherInfo()
        {
            Teacher teacher = Select("\nВыберите преподавателя:", University.Teachers);
            if (teacher != null) Console.WriteLine(teacher.GetInfo());
        }

        private static void ShowCourseInfo()
        {
            Course course = Select("\nВыберите курс:", University.Courses);
            if (course == null) return;

            Console.WriteLine(course.GetInfo());
            Console.WriteLine("  Список студентов:");
            if (course.Students.Count == 0)
                Console.WriteLine("    (пока никого)");
            foreach (var s in course.Students)
                Console.WriteLine($"    - {s.Name}");
        }

        // Полиморфизм: один метод работает с любыми Person
        private static void PrintPeople(string title, IEnumerable<Person> people)
        {
            Console.WriteLine($"\n{title}");
            bool any = false;
            foreach (var p in people)
            {
                Console.WriteLine(p.GetInfo());
                any = true;
            }
            if (!any) Console.WriteLine("(список пуст)");
        }

        private static void ShowAllCourses()
        {
            Console.WriteLine("\nВсе курсы");
            if (University.Courses.Count == 0)
                Console.WriteLine("(список пуст)");
            foreach (var c in University.Courses)
                Console.WriteLine(c.GetInfo() + "\n");
        }

        private static void ShowMenu()
        {
            Console.WriteLine("\nМеню");
            Console.WriteLine(" 1 – Добавить студента");
            Console.WriteLine(" 2 – Добавить преподавателя");
            Console.WriteLine(" 3 – Создать курс");
            Console.WriteLine(" 4 – Записать студента на курс");
            Console.WriteLine(" 5 – Назначить преподавателя на курс");
            Console.WriteLine(" 6 – Курсы конкретного студента");
            Console.WriteLine(" 7 – Информация о студенте");
            Console.WriteLine(" 8 – Информация о преподавателе");
            Console.WriteLine(" 9 – Информация о курсе (со студентами)");
            Console.WriteLine("10 – Все студенты");
            Console.WriteLine("11 – Все преподаватели");
            Console.WriteLine("12 – Все курсы");
            Console.WriteLine(" 0 – Выход");
            Console.Write("Выберите действие: ");
        }

        private static void Main(string[] args)
        {
            while (true)
            {
                ShowMenu();

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Ошибка: введите корректное число.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1: AddStudent(); break;
                        case 2: AddTeacher(); break;
                        case 3: CreateCourse(); break;
                        case 4: EnrollStudent(); break;
                        case 5: AssignTeacher(); break;
                        case 6: ShowStudentCourses(); break;
                        case 7: ShowStudentInfo(); break;
                        case 8: ShowTeacherInfo(); break;
                        case 9: ShowCourseInfo(); break;
                        case 10: PrintPeople("Все студенты", University.Students); break;
                        case 11: PrintPeople("Все преподаватели", University.Teachers); break;
                        case 12: ShowAllCourses(); break;
                        case 0:
                            Console.WriteLine("Выход из программы.");
                            return;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}
