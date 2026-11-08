using CarConfigurator.Models;
using CarConfigurator.Services;
using CarConfigurator.ViewModels.Base;

namespace CarConfigurator.ViewModels
{
    public class Step5ViewModel : ViewModelBase
    {
        private CarConfiguration _config;

        public Step5ViewModel(CarConfiguration config)
        {
            _config = config;
        }

        public string Name
        {
            get => _config.Contact.Name;
            set { _config.Contact.Name = value; OnPropertyChanged(nameof(HasErrors)); }
        }

        public string Phone
        {
            get => _config.Contact.Phone;
            set { _config.Contact.Phone = value; OnPropertyChanged(nameof(HasErrors)); }
        }

        public string Email
        {
            get => _config.Contact.Email;
            set { _config.Contact.Email = value; OnPropertyChanged(nameof(HasErrors)); }
        }

        public bool IsValid => ValidationService.IsNameValid(Name)
                            && ValidationService.IsPhoneValid(Phone)
                            && ValidationService.IsEmailValid(Email);

        public bool HasErrors => !IsValid;

        public string Summary
        {
            get
            {
                double total = PriceCalculator.GetTotal(_config);
                double loan = total * (100 - _config.Credit.Percent) / 100;
                double monthly = CreditCalculator.GetMonthly(loan, _config.Credit.Months);
                var options = string.Join(", ", _config.Options.Where(o => o.IsSelected).Select(o => o.Name));
                return $"Модель: {_config.Model.Name}\nДвигатель: {_config.Engine.Name}\n" +
                       $"Цвет: {_config.Color.Name}\nОпции: {options}\n" +
                       $"Стоимость: {total:N0} ₽\n" +
                       $"Взнос {_config.Credit.Percent}%, срок {_config.Credit.Months} мес.\n" +
                       $"Платёж: {monthly:N0} ₽ в месяц";
            }
        }
    }
}