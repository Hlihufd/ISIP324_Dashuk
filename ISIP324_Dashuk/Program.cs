using System;
using System.Collections.Generic;

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
}

