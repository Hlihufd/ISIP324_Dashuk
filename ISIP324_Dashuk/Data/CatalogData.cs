using ISIP324_Dashuk.Models;
using System.Collections.Generic;

namespace ISIP324_Dashuk.Data
{
    public static class CatalogData
    {
        public static List<CarModel> Models = new List<CarModel>
        {
            new CarModel { Name = "Hyundai Solaris", BasePrice = 1700000 },
            new CarModel { Name = "Skoda Octavia", BasePrice = 2800000 },
            new CarModel { Name = "Kia Sportage", BasePrice = 3500000 },
            new CarModel { Name = "Toyota Camry", BasePrice = 3900000 }
        };

        public static List<Engine> Engines = new List<Engine>
        {
            new Engine { Name = "1.6 л бензин", Extra = 0 },
            new Engine { Name = "2.0 л бензин", Extra = 150000 },
            new Engine { Name = "2.0 л дизель", Extra = 250000 },
            new Engine { Name = "Электро", Extra = 600000 }
        };

        public static List<Color> Colors = new List<Color>
        {
            new Color { Name = "Белый", Extra = 0 },
            new Color { Name = "Серебристый", Extra = 20000 },
            new Color { Name = "Чёрный", Extra = 30000 },
            new Color { Name = "Мятный", Extra = 50000 }
        };

        public static List<Option> Options = new List<Option>
        {
            new Option { Name = "Кожаный салон", Price = 120000 },
            new Option { Name = "Панорамная крыша", Price = 90000 },
            new Option { Name = "Камера заднего вида", Price = 25000 },
            new Option { Name = "Подогрев сидений", Price = 30000 },
            new Option { Name = "Навигация", Price = 40000 }
        };
    }
}