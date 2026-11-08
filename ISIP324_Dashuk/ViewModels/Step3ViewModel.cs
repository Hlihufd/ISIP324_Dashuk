using System;
using System.Collections.Generic;
using System.Linq;
using ISIP324_Dashuk.Models;
using ISIP324_Dashuk.Services;
using ISIP324_Dashuk.ViewModels.Base;

namespace ISIP324_Dashuk.ViewModels
{
    public class Step3ViewModel : ViewModelBase
    {
        public CarConfiguration Config { get; }

        public Step3ViewModel(CarConfiguration config)
        {
            Config = config;
        }

        public double OptionsPrice => PriceCalculator.GetOptionsPrice(Config);
        public double Total => PriceCalculator.GetTotal(Config);

        public string Summary
        {
            get
            {
                var options = string.Join(", ", Config.Options.Where(o => o.IsSelected).Select(o => o.Name));
                return $"Модель: {Config.Model.Name}\nДвигатель: {Config.Engine.Name}\n" +
                       $"Цвет: {Config.Color.Name}\nОпции: {options}";
            }
        }
    }
}