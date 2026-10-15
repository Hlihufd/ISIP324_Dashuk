namespace Roguelike
{
	public class Armor : Item
	{
		public int DefenseBonus { get; private set; }
		public Armor(string name, int defenseBonus, Rarity rarity)
		{
			Name = name; DefenseBonus = defenseBonus; Rarity = rarity; Type = ItemType.Armor;
		}
		public override string GetStatsDescription() => $"Защита: +{DefenseBonus}";
	}
}