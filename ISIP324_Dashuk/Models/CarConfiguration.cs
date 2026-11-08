using System.Data.Common;
using System.Windows.Documents;
using ISIP324_Dashuk.Data;
using System.Collections.Generic;

namespace ISIP324_Dashuk.Models
{
    public class CarConfiguration
    {
        public CarModel Model { get; set; } = CatalogData.Models[0];
        public Engine Engine { get; set; } = CatalogData.Engines[0];
        public Color Color { get; set; } = CatalogData.Colors[0];
        public List<Option> Options { get; set; } = CatalogData.Options;
        public CreditSettings Credit { get; set; } = new CreditSettings();
        public ContactInfo Contact { get; set; } = new ContactInfo();
    }
}