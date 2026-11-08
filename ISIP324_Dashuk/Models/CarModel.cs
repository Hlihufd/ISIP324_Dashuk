namespace ISIP324_Dashuk.Models
{
    public class CarModel
    {
        public string Name { get; set; }
        public double BasePrice { get; set; }

        public override string ToString() => $"{Name} — {BasePrice:N0} ₽";
    }
}