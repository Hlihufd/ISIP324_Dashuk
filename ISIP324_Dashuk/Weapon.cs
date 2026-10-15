namespace Roguelike
{
    public class Weapon : Item
    {
        public int AttackBonus { get; private set; }
        public Weapon(string name, int attackBonus, Rarity rarity)
        {
            Name = name; AttackBonus = attackBonus; Rarity = rarity; Type = ItemType.Weapon;
        }
        public override string GetStatsDescription() => $"Атака: +{AttackBonus}";
    }
}