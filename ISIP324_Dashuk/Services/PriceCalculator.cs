using CarConfigurator.Models;
using System.Linq;

namespace CarConfigurator.Services
{
    public static class PriceCalculator
    {
        public static double GetOptionsPrice(CarConfiguration c) =>
            c.Options.Where(o => o.IsSelected).Sum(o => o.Price);

        public static double GetTotal(CarConfiguration c) =>
            c.Model.BasePrice + c.Engine.Extra + c.Color.Extra + GetOptionsPrice(c);
    }
}