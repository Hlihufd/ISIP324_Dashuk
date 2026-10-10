using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
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


    public sealed class RNG 
    {
        //Singlton
        private static readonly Lazy<RNG> _instance =
            new Lazy<RNG>(() => new RNG());
        //summary
        public static RNG Instance => _instance.Value;
        //polya
        private readonly Random _random;
        //konstruct
        private RNG() { _random = new Random(); }

        public bool RollChance(int percent)
        {
            if(percent <= 0) return false;
            if (percent >= 100) return true;

            int roll = _random.Next(1, 101);
            return roll <= percent;
        }

        public int RandomRange(int min, int max)
        {
            if(min > max)
            {
                int t = max;
                max = min;
                min = t;
            }
            return _random.Next(min, max+1);
        }

        public T RandomChoice<T>(IList<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if(items.Count == 0)
                throw new ArgumentException("Коллекция не должна быть пустой.", nameof(items));

            int index = _random.Next(0, items.Count);
            return items[index];
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }

        public void Shuffle<T>(List<T> list) 
        { 
            if(list == null) throw new ArgumentNullException(nameof(list));

            for(int i = list.Count - 1; i > 0; --i)
            {
                int j = _random.Next(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
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
            var rng = RNG.Instance;

            // 1. Тест RollChance 
            int chests = 0, enemies = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (rng.RollChance(50)) chests++;
                else enemies++;
            }
            Console.WriteLine($"Сундуков: {chests}, Врагов: {enemies}");

            // 2. Тест RandomRange 
            Console.WriteLine($"Величина блока: {rng.RandomRange(70, 100)}%");

            // 3. Тест RandomChoice 
            var races = new List<string> { "Гоблин", "Скелет", "Маг" };
            Console.WriteLine($"Случайный враг: {rng.RandomChoice(races)}");

            // 4. Edge cases
            Console.WriteLine($"Шанс 0%:   {rng.RollChance(0)}");    // всегда false
            Console.WriteLine($"Шанс 100%: {rng.RollChance(100)}");  // всегда true
        }
    }
}
