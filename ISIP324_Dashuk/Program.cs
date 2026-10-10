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
            if (percent <= 0) return false;
            if (percent >= 100) return true;

            int roll = _random.Next(1, 101);
            return roll <= percent;
        }

        public int RandomRange(int min, int max)
        {
            if (min > max)
            {
                int t = max;
                max = min;
                min = t;
            }
            return _random.Next(min, max + 1);
        }

        public T RandomChoice<T>(IList<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
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
            if (list == null) throw new ArgumentNullException(nameof(list));

            for (int i = list.Count - 1; i > 0; --i)
            {
                int j = _random.Next(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }

    public sealed class LootManager
    {
        //Singlton
        private static readonly Lazy<LootManager> _instance =
            new Lazy<LootManager>(() => new LootManager());
        //summary
        public static LootManager Instance => _instance.Value;
        //polya
        private readonly RNG _rng;

        // Множители статов для каждой редкости
        private static readonly Dictionary<Rarity, double> RarityMultipliers =
            new Dictionary<Rarity, double>
            {
                { Rarity.Common,    1.0 },
                { Rarity.Uncommon,  1.3 },
                { Rarity.Rare,      1.7 },
                { Rarity.Mythical,  2.2 },
                { Rarity.Immortal,  2.8 },
                { Rarity.Arcana,    3.5 }
            };
        // Пулы названий для генерации
        private static readonly List<string> WeaponPrefixes = new List<string>
        {
            "Ржавый", "Острый", "Стальной", "Зачарованный",
            "Проклятый", "Святой", "Древний", "Кровавый"
        };

        private static readonly List<string> WeaponNames = new List<string>
        {
            "меч", "клинок", "топор", "кинжал", "копьё", "молот"
        };

        private static readonly List<string> ArmorPrefixes = new List<string>
        {
            "Кожаный", "Стальной", "Укреплённый", "Зачарованный",
            "Древний", "Мифриловый", "Драконий", "Святой"
        };

        private static readonly List<string> ArmorNames = new List<string>
        {
            "нагрудник", "кольчуга", "латные доспехи", "мантия", "кираса", "щит"
        };

        // Конструктор
        private LootManager()
        {
            _rng = RNG.Instance;

        }

        public Item GenerateChestLoot(int playerMaxHP)
        {
            int roll = _rng.RandomRange(1, 100);
            if (roll <= 33)
                return GeneratePotion(playerMaxHP);
            if (roll <= 66)
                return GenerateWeapon();

            return GenerateArmor();
        }
        /// Определяет редкость предмета на основе весов из комментариев к enum Rarity
        private Rarity RollRarity()
        {
            double roll = _rng.NextDouble() * 100.0; // Диапазон [0.0, 99.999...)

            if (roll < 50.0) return Rarity.Common;
            if (roll < 75.0) return Rarity.Uncommon;      // 50 + 25
            if (roll < 90.0) return Rarity.Rare;          // 75 + 15
            if (roll < 97.0) return Rarity.Mythical;      // 90 + 7
            if (roll < 99.9) return Rarity.Immortal;      // 97 + 2.9
            return Rarity.Arcana;                         // Оставшиеся 0.1
        }

        /// Генерирует зелье, восстанавливающее от 30% до 60% от максимального ХП игрока
        private Item GeneratePotion(int playerMaxHP)
        {
            int minHeal = Math.Max(1, (int)(playerMaxHP * 0.3));
            int maxHeal = Math.Max(minHeal, (int)(playerMaxHP * 0.6));
            int healAmount = _rng.RandomRange(minHeal, maxHeal);

            return new Potion(healAmount);
        }
        /// Генерирует оружие с базовым уроном 3-8, умноженным на множитель редкости
        private Item GenerateWeapon()
        {
            Rarity rarity = RollRarity();
            int baseAttack = _rng.RandomRange(3, 8);
            double multiplier = RarityMultipliers[rarity];

            // Округляем до ближайшего целого для чистоты значений
            int finalAttack = (int)Math.Round(baseAttack * multiplier);

            string prefix = _rng.RandomChoice(WeaponPrefixes);
            string name = _rng.RandomChoice(WeaponNames);

            // Делаем первую букву имени заглавной для красоты
            string formattedName = $"{prefix} {name}";

            return new Weapon(formattedName, finalAttack, rarity);
        }
        /// Генерирует броню с базовой защитой 2-6, умноженной на множитель редкости
        private Item GenerateArmor()
        {
            Rarity rarity = RollRarity();
            int baseDefense = _rng.RandomRange(2, 6);
            double multiplier = RarityMultipliers[rarity];

            int finalDefense = (int)Math.Round(baseDefense * multiplier);

            string prefix = _rng.RandomChoice(ArmorPrefixes);
            string name = _rng.RandomChoice(ArmorNames);

            string formattedName = $"{prefix} {name}";

            return new Armor(formattedName, finalDefense, rarity);
        }
    }




    public abstract class Item
    {
        public string Name { get; protected set; }
        public ItemType Type { get; protected set; }
        public Rarity Rarity { get; protected set; }


        public abstract string GetStatsDescription();

        public string GetRarityColor()
        {
            switch(Rarity)
            {
                case Rarity.Common: return "\x1b[37m"; // Белый
                case Rarity.Uncommon: return "\x1b[32m"; // Зеленый
                case Rarity.Rare: return "\x1b[34m"; // Синий
                case Rarity.Mythical: return "\x1b[1;38;5;93m"; // Фиолетовый
                case Rarity.Immortal: return "\x1b[1;38;5;220m"; // Золотой
                case Rarity.Arcana: return "\x1b[31m"; // Красный
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
        public override string ToString()
        {
            return $"{GetRarityColor()}[{GetRarityName()}] {Name}\x1b[0m";
        }
    }

    public class Weapon : Item
    {
        public int AttackBonus {  get; private set; }
        public Weapon(string name, int attackBonus, Rarity rarity)
        {
            Name = name;
            AttackBonus = attackBonus;
            Rarity = rarity;
            Type = ItemType.Weapon;
        }
        public override string GetStatsDescription()
        {
            return $"Атака: +{AttackBonus}";
        }
    }
    public class Armor : Item
    {
        public int DefenseBonus { get; private set; }
        public Armor(string name, int defenseBonus, Rarity rarity)
        {
            Name = name;
            DefenseBonus = defenseBonus;
            Rarity = rarity;
            Type = ItemType.Armor;
        }

        public override string GetStatsDescription()
        {
            return $"Защита: +{DefenseBonus}";
        }
    }

    public class Potion : Item 
    {
        public int HPRecovery {  get; private set; }

        public Potion(int healAmount)
        {
            Name = "Лечебное зелье";
            HPRecovery = healAmount;
            Rarity = Rarity.Common;
            Type = ItemType.Potion;
        }

        public override string GetStatsDescription()
        {
            return $"Лечение: {HPRecovery} HP";
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

            // 5. Тест LootManager (генерируем 20 предметов из сундуков)
            Console.WriteLine("\nТест генерации лута (20 сундуков)");
            int testPlayerMaxHP = 100;

            for (int i = 1; i <= 20; i++)
            {
                Item loot = LootManager.Instance.GenerateChestLoot(testPlayerMaxHP);
                Console.WriteLine($"Сундук {i,2}: {loot.ToString(),-35} | {loot.GetStatsDescription()}");
            }

            // 6. Проверка цвета и запис оружия
            Weapon w1 = new Weapon("ddd", 3, Rarity.Common);
            Weapon w2 = new Weapon("ddd", 3, Rarity.Uncommon);
            Weapon w3 = new Weapon("ddd", 3, Rarity.Rare);
            Weapon w4 = new Weapon("ddd", 3, Rarity.Mythical);
            Weapon w5 = new Weapon("ddd", 3, Rarity.Immortal);
            Weapon w6 = new Weapon("ddd", 3, Rarity.Arcana);
            Console.WriteLine("\n" + w1+"\n" + w2 +"\n" + w3 +"\n" + w4 + "\n" + w5 + "\n" + w6);
        }
    }
}
