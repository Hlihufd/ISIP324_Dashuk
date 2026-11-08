using ISIP324_Dashuk.Data;
using ISIP324_Dashuk.Models;
using ISIP324_Dashuk.Services;
using ISIP324_Dashuk.ViewModels.Base;
using System.Collections.Generic;
namespace ISIP324_Dashuk.ViewModels
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