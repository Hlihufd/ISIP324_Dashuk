using System;

namespace Roguelike
{
	public class Player : Entity
	{
		public Armor EquippedArmor { get; set; }
		public Weapon EquippedWeapon { get; set; }
		public PlayerAction playerAction { get; set; }

		public Player(string name, double current_HP, double max_HP, int basic_Attack, int basic_Protection) :
			base(name, current_HP, max_HP, basic_Attack, basic_Protection)
		{ }

		public void EquipWeapon(Weapon weapon)
		{
			EquippedWeapon = weapon;
			Console.WriteLine($"Вы экипировали оружие: {weapon.Name} (+{weapon.AttackBonus} к атаке)");
		}

		public void EquipArmor(Armor armor)
		{
			EquippedArmor = armor;
			Console.WriteLine($"Вы надели броню: {armor.Name} (+{armor.DefenseBonus} к защите)");
		}

		public override double GetTotalAttack() => Basic_Attack + (EquippedWeapon?.AttackBonus ?? 0);
		public override double GetTotalProtection() => Basic_Protection + (EquippedArmor?.DefenseBonus ?? 0);

		public override void PerformAttack(Entity target)
		{
			double damage = GetTotalAttack();
			Console.WriteLine($"\n[Ход Игрока] {Name} атакует {target.Name} с помощью {(EquippedWeapon != null ? EquippedWeapon.Name : "кулаков")}!");
			target.TakeDamage(damage);
		}
	}
}