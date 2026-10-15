using System;

namespace Roguelike
{
	public class Goblin : Entity
	{
		public int CritChancePercent { get; }
		public Goblin(string name, double current_HP, double max_HP, double basic_Attack, double basic_Protection, int CritChancePercent = 35) :
			base(name, current_HP, max_HP, basic_Attack, basic_Protection)
		{ this.CritChancePercent = CritChancePercent; }

		public override void PerformAttack(Entity target)
		{
			bool isCrit = RNG.Instance.RollChance(CritChancePercent);
			double damage = GetTotalAttack() * (isCrit ? 2 : 1);
			if (isCrit) Console.WriteLine($"\n[Ход Врага] {Name} наносит КРИТИЧЕСКИЙ удар!");
			else Console.WriteLine($"\n[Ход Врага] {Name} атакует!");
			target.TakeDamage(damage);
		}
	}

	public class VVG : Goblin
	{
		public VVG(double current_HP, double max_HP, double basic_Attack, double basic_Protection, int CritChancePercent, string name = "ВВГ") :
			base(name, current_HP * 2.0, max_HP * 2.0, basic_Attack * 1.5, basic_Protection * 1.2, CritChancePercent + 10)
		{ }
		public override void PerformAttack(Entity target)
		{
			bool isCrit = RNG.Instance.RollChance(CritChancePercent);
			double damage = GetTotalAttack() * (isCrit ? 2 : 1);
			if (isCrit) Console.WriteLine($"\n[Ход Босса] {Name} наносит КРИТИЧЕСКИЙ удар!");
			else Console.WriteLine($"\n[Ход Босса] {Name} атакует!");
			target.TakeDamage(damage);
		}
	}
}