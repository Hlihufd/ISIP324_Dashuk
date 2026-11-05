using CarConfigurator.Models;
namespace CarConfigurator.Services
{
    public static class PriceCalculator
    {
        public static double GetOptionsPrice(CarConfiguration c) =>
            c.Model.BasePrice + c.Engine.Extra + c.Color.Extra + GetOptionsPrice(c);
    }
}