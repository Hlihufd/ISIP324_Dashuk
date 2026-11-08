using ISIP324_Dashuk.Models;
using ISIP324_Dashuk.Services;
using ISIP324_Dashuk.ViewModels.Base;

namespace ISIP324_Dashuk.ViewModels
{
    public class Step4ViewModel : ViewModelBase
    {
        private CarConfiguration _config;

        public Step4ViewModel(CarConfiguration config)
        {
            _config = config;
        }

        public int Percent
        {
            get => _config.Credit.Percent;
            set { _config.Credit.Percent = value; OnPropertyChanged(""); }
        }

        public int Months
        {
            get => _config.Credit.Months;
            set { _config.Credit.Months = value; OnPropertyChanged(""); }
        }

        public double Total => PriceCalculator.GetTotal(_config);
        public double DownPayment => Total * Percent / 100;
        public double Loan => Total - DownPayment;
        public double Monthly => CreditCalculator.GetMonthly(Loan, Months);
    }
}