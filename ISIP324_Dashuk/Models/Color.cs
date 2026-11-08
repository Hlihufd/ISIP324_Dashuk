namespace ISIP324_Dashuk.Models
{
    public class Color
    {
        public string Name { get; set; }
        public double Extra { get; set; }
        public override string ToString() => $"{Name}  +{Extra:N0} руб";
    }
}