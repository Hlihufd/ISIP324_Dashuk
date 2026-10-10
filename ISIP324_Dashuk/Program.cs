using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public enum Rarity
    {
        Common = 1, // 50
        Uncommon, // 25
        Rare, // 15
        Mythical, // 7
        Immortal, // 2.9
        Arcana // 0.1
    }
    public enum ItemType
    {
        Potion = 1,
        Weapon,
        Armor
    }
    public enum EnemyRace
    {
        Goblin = 1,
        Skeleton,
        Mage
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


    public abstract class Item
    {
        protected string Name { get; }
        protected ItemType _item;
        protected Rarity _rarity;

        public Item(ItemType item, string name, Rarity rarity)
        {
            _item = item;
            _rarity = rarity;
            Name = name;
        }
    }

    public class Weapon : Item
    {
        public float Attack_Bonus {  get; set; }
        public int Rareness_Multiplier { get; set; }

        public Weapon(float attack_Bonus, int rareness_Multiplier, ItemType item, string name, Rarity rarity) : base(item, name, rarity)
        {
            Attack_Bonus = attack_Bonus;
            Rareness_Multiplier = rareness_Multiplier;
        }
    }
    public class Armor : Item
    {
        public float Bonus_To_Defense { get; set; }
        public int Rareness_Multiplier { get; set; }

        public Armor(float bonus_to_Defense, int rareness_Multiplier, ItemType item, string name, Rarity rarity) : base(item, name, rarity)
        {
            Bonus_To_Defense = bonus_to_Defense;
            Rareness_Multiplier = rareness_Multiplier;
        }
    }

    public class Poition : Item 
    {
        public float HPRecovery {  get; set; }

        public Poition(float hpRecovery, ItemType item, string name, Rarity rarity) : base(item, name, rarity)
        {
            HPRecovery = hpRecovery;
        }
    }

    public abstract class Entity
    {
        protected string Name { get; set; }
        protected float Current_HP {  get; set; }
        protected float Max_HP { get; set; }
        protected int Basic_Attack { get; set; }
        protected int Basic_Protection { get; set; }

        public Entity(string name, float current_HP, float max_HP, int basic_Attack, int basic_Protection)
        {
            Name = name;
            Current_HP = current_HP;
            Max_HP = max_HP;
            Basic_Attack = basic_Attack;
            Basic_Protection = basic_Protection;
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }
}

