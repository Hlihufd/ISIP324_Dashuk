using System;

namespace Roguelike
{
    public class Mage : Entity
    {
        public int FreezeChancePercent { get; }
        public Mage(string name, double current_HP, double max_HP, double basic_Attack, double basic_Protection, int FreezeChancePercent = 20) :
            base(name, current_HP, max_HP, basic_Attack, basic_Protection)
        { this.FreezeChancePercent = FreezeChancePercent; }

        public override void PerformAttack(Entity target)
        {
            Console.WriteLine($"\n[Ход Врага] {Name} кастует заклинание!");
            target.TakeDamage(GetTotalAttack());

            if (RNG.Instance.RollChance(FreezeChancePercent))
            {
                target.IsFrosen = true;
                Console.WriteLine($"\x1b[36m{Name} накладывает «Заморозку»! {target.Name} пропустит следующий ход.\x1b[0m");
            }
        }
    }

    public class Archmage : Mage
    {
        public Archmage(string name, double current_HP, double max_HP, double basic_Attack, double basic_Protection, int FreezeChancePercent = 20) :
            base(name, current_HP * 1.8, max_HP * 1.8, basic_Attack * 1.6, basic_Protection * 1.1, FreezeChancePercent + 10)
        { }

        public override void PerformAttack(Entity target)
        {
            Console.WriteLine($"\n[Ход Босса] {Name} кастует заклинание!");
            target.TakeDamage(GetTotalAttack());

            if (RNG.Instance.RollChance(FreezeChancePercent))
            {
                target.IsFrosen = true;
                Console.WriteLine($"\x1b[36m{Name} накладывает «Заморозку»! {target.Name} пропустит следующий ход.\x1b[0m");
            }
        }
    }
}