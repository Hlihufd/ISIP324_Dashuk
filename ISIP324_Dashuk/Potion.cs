namespace Roguelike
{
	public class Potion : Item
	{
		public int HPRecovery { get; private set; }
		public Potion(int healAmount)
		{
			Name = "Лечебное зелье"; HPRecovery = healAmount; Rarity = Rarity.Common; Type = ItemType.Potion;
		}
		public override string GetStatsDescription() => $"Лечение: {HPRecovery} HP";
	}
}