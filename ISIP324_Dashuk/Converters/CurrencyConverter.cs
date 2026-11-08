using System.Globalization;
using System.Windows.Data;
using System;

namespace CarConfigurator.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        // текст перед ценой
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => $"{parameter}{value:N0} руб";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}﻿
