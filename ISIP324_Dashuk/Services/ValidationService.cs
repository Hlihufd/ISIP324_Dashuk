using System.Linq;
using System.Text.RegularExpressions;

namespace CarConfigurator.Services
{
    public static class ValidationService
    {
        public static bool IsNameValid(string name) => name.Trim().Length >= 2;
        public static bool IsPhoneValid(string phone) => phone.Length >= 10 && phone.All(char.IsDigit());
        public static bool IsEmailValid(string email) => Regax.IsMatch(email, @"^\S+@\S+\.\S+$");
    }
}