using System;

namespace Roguelike
{
    public class Skeleton : Entity
    {
        public Skeleton(string name, double current_HP, double max_HP, double basic_Attack, double basic_Protection) :
            base(name, current_HP, max_HP, basic_Attack, basic_Protection)
        { }

        public override void PerformAttack(Entity target)
        {
            Console.WriteLine($"\n[Ход Врага] {Name} проводит пронизывающую атаку, ИГНОРИРУЯ броню!");
            target.TakeDamage(GetTotalAttack(), ignoreArmor: true);
        }
    }

    public class Kowalski : Skeleton
    {
        public Kowalski(double current_HP, double max_HP, double basic_Attack, double basic_Protection, string name = "Ковальский") :
            base(name, current_HP * 2.5, max_HP * 2.5, basic_Attack * 1.3, basic_Protection * 1.4)
        { }
        public override void PerformAttack(Entity target)
        {
            Console.WriteLine($"\n[Ход Босса] {Name} проводит пронизывающую атаку, ИГНОРИРУЯ броню!");
            target.TakeDamage(GetTotalAttack(), ignoreArmor: true);
        }
    }

    public class Pestov : Skeleton
    {
        public int FreezeChancePercent { get; } = 35;
        public Pestov(double current_HP, double max_HP, double basic_Attack, double basic_Protection, string name = "Пестов С--") :
            base(name, current_HP * 1.3, max_HP * 1.3, basic_Attack * 1.8, basic_Protection * 0.6)
        { }
        public override void PerformAttack(Entity target)
        {
            Console.WriteLine($"\n[Ход Босса] {Name} проводит пронизывающую атаку, ИГНОРИРУЯ броню!");
            target.TakeDamage(GetTotalAttack(), ignoreArmor: true);
            if (RNG.Instance.RollChance(FreezeChancePercent))
            {
                target.IsFrosen = true;
                Console.WriteLine($"\x1b[36m{Name} накладывает «Заморозку»! {target.Name} пропустит следующий ход.\x1b[0m");
            }
        }
    }
}