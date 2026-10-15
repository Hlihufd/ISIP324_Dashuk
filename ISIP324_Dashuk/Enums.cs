namespace Roguelike
{
	public enum Rarity
	{
		Common = 1, // 45%
		Uncommon,   // 25%
		Rare,       // 15%
		Mythical,   // 8%
		Immortal,   // 5%
		Arcana      // 2%
	}

	public enum ItemType
	{
		Potion = 1,
		Weapon,
		Armor
	}

	public enum BossId
	{
		VVG = 1, // раса Гоблин
		Kovalsky, // раса Скелет
		Archmage_C, // раса Маг
		Pestov_C // раса Скелет
	}

	public enum PlayerAction
	{
		To_Attack = 1,
		To_Defend
	}
}