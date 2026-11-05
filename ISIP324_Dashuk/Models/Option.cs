namespace CarConfigurator.Models
{
    public class Option
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsSelected { get; set; }

        public override string ToString() => $"{Name} +{Price:N0} руб";
    }
}