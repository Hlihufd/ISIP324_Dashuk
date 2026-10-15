namespace Roguelike
{
	public abstract class Item
	{
		public string Name { get; protected set; }
		public ItemType Type { get; protected set; }
		public Rarity Rarity { get; protected set; }

		public abstract string GetStatsDescription();

		public string GetRarityColor()
		{
			switch (Rarity)
			{
				case Rarity.Common: return "\x1b[37m";
				case Rarity.Uncommon: return "\x1b[32m";
				case Rarity.Rare: return "\x1b[34m";
				case Rarity.Mythical: return "\x1b[1;38;5;93m";
				case Rarity.Immortal: return "\x1b[1;38;5;220m";
				case Rarity.Arcana: return "\x1b[31m";
				default: return "\x1b[0m";
			}
		}

		public string GetRarityName()
		{
			switch (Rarity)
			{
				case Rarity.Common: return "Обычное";
				case Rarity.Uncommon: return "Необычное";
				case Rarity.Rare: return "Редкое";
				case Rarity.Mythical: return "Мифическое";
				case Rarity.Immortal: return "Имортальное";
				case Rarity.Arcana: return "Аркана";
				default: return "???";
			}
		}

		public override string ToString() => $"{GetRarityColor()}[{GetRarityName()}] {Name}\x1b[0m";
	}
}