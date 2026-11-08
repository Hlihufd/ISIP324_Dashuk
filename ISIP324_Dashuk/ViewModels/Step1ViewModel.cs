using CarConfigurator.Data;
using CarConfigurator.Models;
using CarConfigurator.Services;
using CarConfigurator.ViewModels.Base;

namespace CarConfigurator.ViewModels
{
    public class Step1ViewModel : ViewModelBase
    {
        private CarConfiguration _config;

        public Step1ViewModel(CarConfiguration config)
        {
            _config = config;
        }

        public List<CarModel> Models => CatalogData.Models;
        public List<Engine> Engines => CatalogData.Engines;

        public CarModel SelectedModel
        {
            get => _config.Model;
            set { _config.Model = value; OnPropertyChanged(""); }
        }

        public Engine SelectedEngine
        {
            get => _config.Engine;
            set { _config.Engine = value; OnPropertyChanged(""); }
        }

        public double Total => PriceCalculator.GetTotal(_config);
    }
}