using CarConfigurator.Data;
using CarConfigurator.Models;
using CarConfigurator.Services;
using CarConfigurator.ViewModels.Base;
using System.Collections.Generic;

namespace CarConfigurator.ViewModels
{
    public class Step2ViewModel : ViewModelBase
    {
        private CarConfiguration _config;

        public Step2ViewModel(CarConfiguration config)
        {
            _config = config;
        }

        public List<Color> Colors => CatalogData.Colors;
        public List<Option> Options => _config.Options;

        public Color SelectedColor
        {
            get => _config.Color;
            set { _config.Color = value; OnPropertyChanged(""); }
        }

        public double OptionsPrice => PriceCalculator.GetOptionsPrice(_config);
        public double Total => PriceCalculator.GetTotal(_config);

        // вызывается при клике на галочку опции
        public void Update() => OnPropertyChanged("");
    }
}