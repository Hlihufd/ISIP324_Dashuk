using System;

namespace Roguelike
{
    public abstract class Entity
    {
        public string Name { get; protected set; }
        public double Current_HP { get; set; }
        public double Max_HP { get; protected set; }
        public double Basic_Attack { get; protected set; }
        public double Basic_Protection { get; protected set; }

        public bool IsAlive => Current_HP > 0;
        public bool IsFrosen { get; set; } = false;

        protected Entity(string name, double current_HP, double max_HP, double basic_Attack, double basic_Protection)
        {
            Name = name;
            Current_HP = Math.Min(current_HP, max_HP);
            Max_HP = max_HP;
            Basic_Attack = basic_Attack;
            Basic_Protection = basic_Protection;
        }

        public virtual double GetTotalAttack() => Basic_Attack;
        public virtual double GetTotalProtection() => Basic_Protection;

        public virtual void TakeDamage(double rawDamage, bool ignoreArmor = false)
        {
            double effectiveDefense = ignoreArmor ? 0 : GetTotalProtection();
            double actualDamage = Math.Max(1, rawDamage - effectiveDefense);
            Current_HP = Math.Max(0, Current_HP - actualDamage);
            Console.WriteLine($"{Name} получает {actualDamage:F1} урона! (Осталось HP: {Current_HP:F1}/{Max_HP:F1})");
        }

        public virtual void Heal(int amount)
        {
            Current_HP = Math.Min(Max_HP, Current_HP + amount);
            Console.WriteLine($"{Name} восстанавливает {amount} HP! (Текущее HP: {Current_HP:F1}/{Max_HP:F1})");
        }

        public abstract void PerformAttack(Entity target);
    }
}