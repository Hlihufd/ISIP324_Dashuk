using System;
using System.Collections.Generic;

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

    public sealed class RNG
    {
        private static readonly Lazy<RNG> _instance = new Lazy<RNG>(() => new RNG());
        public static RNG Instance => _instance.Value;
        private readonly Random _random;

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
            if (min > max) { int t = max; max = min; min = t; }
            return _random.Next(min, max + 1);
        }

        public T RandomChoice<T>(IList<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (items.Count == 0) throw new ArgumentException("Коллекция не должна быть пустой.", nameof(items));
            return items[_random.Next(0, items.Count)];
        }

        public double NextDouble() => _random.NextDouble();

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
        private static readonly Lazy<LootManager> _instance = new Lazy<LootManager>(() => new LootManager());
        public static LootManager Instance => _instance.Value;
        private readonly RNG _rng;

        private static readonly Dictionary<Rarity, double> RarityMultipliers = new Dictionary<Rarity, double>
        {
            { Rarity.Common, 1.0 }, { Rarity.Uncommon, 1.3 }, { Rarity.Rare, 1.7 },
            { Rarity.Mythical, 2.2 }, { Rarity.Immortal, 2.8 }, { Rarity.Arcana, 3.5 }
        };

        private static readonly List<string> WeaponPrefixes = new List<string> { "Ржавый", "Острый", "Стальной", "Зачарованный", "Проклятый", "Святой", "Древний", "Кровавый" };
        private static readonly List<string> WeaponNames = new List<string> { "меч", "клинок", "топор", "кинжал", "копьё", "молот" };
        private static readonly List<string> ArmorPrefixes = new List<string> { "Кожаный", "Стальной", "Укреплённый", "Зачарованный", "Древний", "Мифриловый", "Драконий", "Святой" };
        private static readonly List<string> ArmorNames = new List<string> { "нагрудник", "кольчуга", "латные доспехи", "мантия", "кираса", "щит" };

        private LootManager() { _rng = RNG.Instance; }

        public Item GenerateChestLoot(int playerMaxHP)
        {
            int roll = _rng.RandomRange(1, 100);
            if (roll <= 33) return GeneratePotion(playerMaxHP);
            if (roll <= 66) return GenerateWeapon();
            return GenerateArmor();
        }

        private Rarity RollRarity()
        {
            double roll = _rng.NextDouble() * 100.0;
            if (roll < 45.0) return Rarity.Common;
            if (roll < 70.0) return Rarity.Uncommon;
            if (roll < 85.0) return Rarity.Rare;
            if (roll < 93.0) return Rarity.Mythical;
            if (roll < 98.0) return Rarity.Immortal;
            return Rarity.Arcana;
        }

        private Item GeneratePotion(int playerMaxHP)
        {
            int minHeal = Math.Max(1, (int)(playerMaxHP * 0.3));
            int maxHeal = Math.Max(minHeal, (int)(playerMaxHP * 0.6));
            return new Potion(_rng.RandomRange(minHeal, maxHeal));
        }

        private Item GenerateWeapon()
        {
            Rarity rarity = RollRarity();
            int finalAttack = (int)Math.Round(_rng.RandomRange(3, 8) * RarityMultipliers[rarity]);
            return new Weapon($"{_rng.RandomChoice(WeaponPrefixes)} {_rng.RandomChoice(WeaponNames)}", finalAttack, rarity);
        }

        private Item GenerateArmor()
        {
            Rarity rarity = RollRarity();
            int finalDefense = (int)Math.Round(_rng.RandomRange(2, 6) * RarityMultipliers[rarity]);
            return new Armor($"{_rng.RandomChoice(ArmorPrefixes)} {_rng.RandomChoice(ArmorNames)}", finalDefense, rarity);
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

    public class Weapon : Item
    {
        public int AttackBonus { get; private set; }
        public Weapon(string name, int attackBonus, Rarity rarity)
        {
            Name = name; AttackBonus = attackBonus; Rarity = rarity; Type = ItemType.Weapon;
        }
        public override string GetStatsDescription() => $"Атака: +{AttackBonus}";
    }

    public class Armor : Item
    {
        public int DefenseBonus { get; private set; }
        public Armor(string name, int defenseBonus, Rarity rarity)
        {
            Name = name; DefenseBonus = defenseBonus; Rarity = rarity; Type = ItemType.Armor;
        }
        public override string GetStatsDescription() => $"Защита: +{DefenseBonus}";
    }

    public class Potion : Item
    {
        public int HPRecovery { get; private set; }
        public Potion(int healAmount)
        {
            Name = "Лечебное зелье"; HPRecovery = healAmount; Rarity = Rarity.Common; Type = ItemType.Potion;
        }
        public override string GetStatsDescription() => $"Лечение: {HPRecovery} HP";
    }

    public abstract class Entity
    {
        public string Name { get; protected set; }
        public float Current_HP { get;  set; }
        public float Max_HP { get; protected set; }
        public int Basic_Attack { get; protected set; }
        public int Basic_Protection { get; protected set; }

        public bool IsAlive => Current_HP > 0;
        public bool IsFrosen { get; set; } = false;

        protected Entity(string name, float current_HP, float max_HP, int basic_Attack, int basic_Protection)
        {
            Name = name;
            Current_HP = Math.Min(current_HP, max_HP);
            Max_HP = max_HP;
            Basic_Attack = basic_Attack;
            Basic_Protection = basic_Protection;
        }

        public virtual int GetTotalAttack() => Basic_Attack;
        public virtual int GetTotalProtection() => Basic_Protection;

        public virtual void TakeDamage(int rawDamage, bool ignoreArmor = false)
        {
            int effectiveDefense = ignoreArmor ? 0 : GetTotalProtection();
            int actualDamage = Math.Max(1, rawDamage - effectiveDefense);
            Current_HP = Math.Max(0, Current_HP - actualDamage);
            Console.WriteLine($"{Name} получает {actualDamage} урона! (Осталось HP: {Current_HP}/{Max_HP})");
        }

        public virtual void Heal(int amount)
        {
            Current_HP = Math.Min(Max_HP, Current_HP + amount);
            Console.WriteLine($"{Name} восстанавливает {amount} HP! (Текущее HP: {Current_HP}/{Max_HP})");
        }

        public abstract void PerformAttack(Entity target);
    }

    public class Player : Entity
    {
        public Armor EquippedArmor { get; set; }
        public Weapon EquippedWeapon { get; set; }
        public PlayerAction playerAction { get; set; }

        public Player(string name, float current_HP, float max_HP, int basic_Attack, int basic_Protection) :
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

        public override int GetTotalAttack() => Basic_Attack + (EquippedWeapon?.AttackBonus ?? 0);
        public override int GetTotalProtection() => Basic_Protection + (EquippedArmor?.DefenseBonus ?? 0);

        public override void PerformAttack(Entity target)
        {
            int damage = GetTotalAttack();
            Console.WriteLine($"\n[Ход Игрока] {Name} атакует {target.Name} " +
                $"с помощью {(EquippedWeapon != null ? EquippedWeapon.Name : "кулаков")}!");
            target.TakeDamage(damage);
        }
    }

    public class Goblin : Entity
    {
        public int CritChancePercent { get; } = 30;
        public Goblin(string name, float current_HP, float max_HP, int basic_Attack, int basic_Protection) :
            base(name, current_HP, max_HP, basic_Attack, basic_Protection)
        { }

        public override void PerformAttack(Entity target)
        {
            bool isCrit = RNG.Instance.RollChance(CritChancePercent);
            int damage = GetTotalAttack() * (isCrit ? 2 : 1);
            if (isCrit) Console.WriteLine($"\n[Ход Врага] {Name} наносит КРИТИЧЕСКИЙ удар!");
            else Console.WriteLine($"\n[Ход Врага] {Name} атакует!");
            target.TakeDamage(damage);
        }
    }

    public class Skeleton : Entity
    {
        public Skeleton(string name, float current_HP, float max_HP, int basic_Attack, int basic_Protection) :
            base(name, current_HP, max_HP, basic_Attack, basic_Protection)
        { }

        public override void PerformAttack(Entity target)
        {
            Console.WriteLine($"\n[Ход Врага] {Name} проводит пронизывающую атаку, ИГНОРИРУЯ броню!");
            target.TakeDamage(GetTotalAttack(), ignoreArmor: true);
        }
    }

    public class Mage : Entity
    {
        public int FreezeChancePercent { get; } = 20;
        public Mage(string name, float current_HP, float max_HP, int basic_Attack, int basic_Protection) :
            base(name, current_HP, max_HP, basic_Attack, basic_Protection)
        { }

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

    internal class Program
    {
        // Метод для проведения одного тестового боя
        static void RunBattleTest(Player player, Entity enemy)
        {
            Console.WriteLine($"\nНАЧАЛО БОЯ: {player.Name} против {enemy.Name}");
            int round = 1;

            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine($"\nРаунд {round}");

                player.PerformAttack(enemy);
                if (!enemy.IsAlive)
                {
                    Console.WriteLine($"\n{enemy.Name} повержен!");
                    Item loot = LootManager.Instance.GenerateChestLoot((int)player.Max_HP);
                    Console.WriteLine($"Выпал лут: {loot} | {loot.GetStatsDescription()}");
                    break;
                }

                if (enemy.IsFrosen)
                {
                    Console.WriteLine($"\x1b[36m{enemy.Name} заморожен и пропускает ход!\x1b[0m");
                    enemy.IsFrosen = false;
                }
                else
                {
                    enemy.PerformAttack(player);
                    if (!player.IsAlive)
                    {
                        Console.WriteLine($"\n {player.Name} Поражение. ");
                        break;
                    }
                }

                Console.WriteLine($"Статус: Игрок HP: {player.Current_HP}/{player.Max_HP} | Враг HP: {enemy.Current_HP}/{enemy.Max_HP}");
                round++;
            }
        }

        // Метод для RNG-цикла тестирования
        static void RunRngLoopTest()
        {
            Console.WriteLine("\nЗАПУСК RNG ЦИКЛА ТЕСТИРОВАНИЯ (5 СЛУЧАЙНЫХ БОЕВ)");
            var rng = RNG.Instance;

            var enemyFactories = new List<Func<Entity>>
            {
                () => new Goblin("Гоблин-разведчик", 60, 60, 12, 2),
                () => new Skeleton("Древний скелет", 80, 80, 18, 4),
                () => new Mage("Темный маг", 70, 70, 20, 3)
            };

            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"\n>>> Тестовый бой #{i} <<<");

                Player player = new Player($"Герой-Тестовик {i}", 100, 100, 15, 5);
                if (rng.RollChance(50))
                {
                    player.EquipWeapon(new Weapon("Тренировочный меч", 5, Rarity.Common));
                }

                Entity randomEnemy = rng.RandomChoice(enemyFactories)();
                int randomHp = rng.RandomRange((int)(randomEnemy.Max_HP * 0.8f), (int)randomEnemy.Max_HP);
                randomEnemy.Current_HP = randomHp;

                RunBattleTest(player, randomEnemy);
                Console.WriteLine(new string('-', 40));
            }
            Console.WriteLine("\nRNG ЦИКЛ ЗАВЕРШЕН");
        }

        static void RunOriginalRngTests()
        {
            var rng = RNG.Instance;
            Console.WriteLine("Тест 1: RollChance (100000 итераций)");
            int chests = 0, enemies = 0;
            for (int i = 0; i < 100000; i++)
            {
                if (rng.RollChance(50)) chests++;
                else enemies++;
            }
            Console.WriteLine($"Сундуков: {chests}, Врагов: {enemies}");

            Console.WriteLine("\nТест 2: RandomRange");
            Console.WriteLine($"Величина блока: {rng.RandomRange(70, 100)}%");

            Console.WriteLine("\nТест 3: RandomChoice");
            var races = new List<string> { "Гоблин", "Скелет", "Маг" };
            Console.WriteLine($"Случайный враг: {rng.RandomChoice(races)}");

            Console.WriteLine("\nТест 4: Edge cases");
            Console.WriteLine($"Шанс 0%:   {rng.RollChance(0)}");
            Console.WriteLine($"Шанс 100%: {rng.RollChance(100)}");
        }

        static void RunLootTests()
        {
            Console.WriteLine("\nТест 5: Генерация лута (50 сундуков)");
            int testPlayerMaxHP = 100;

            for (int i = 1; i <= 50; i++)
            {
                Item loot = LootManager.Instance.GenerateChestLoot(testPlayerMaxHP);
                Console.WriteLine($"Сундук {i,2}: {loot.ToString(),-35} | {loot.GetStatsDescription()}");
            }
        }

        static void RunRarityColorTests()
        {
            Console.WriteLine("\nТест 6: Проверка цвета и названий оружия");
            Weapon w1 = new Weapon("Меч новичка", 3, Rarity.Common);
            Weapon w2 = new Weapon("Клинок ветра", 4, Rarity.Uncommon);
            Weapon w3 = new Weapon("Топор бури", 6, Rarity.Rare);
            Weapon w4 = new Weapon("Кинжал тени", 8, Rarity.Mythical);
            Weapon w5 = new Weapon("Копье судьбы", 10, Rarity.Immortal);
            Weapon w6 = new Weapon("Молот богов", 12, Rarity.Arcana);

            Console.WriteLine(w1.ToString());
            Console.WriteLine(w2.ToString());
            Console.WriteLine(w3.ToString());
            Console.WriteLine(w4.ToString());
            Console.WriteLine(w5.ToString());
            Console.WriteLine(w6.ToString());
        }

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("1. RNG (Шансы, Диапазоны, Выбор)");
                Console.WriteLine("2. LootManager (50 сундуков)");
                Console.WriteLine("3. Редкость и цвета предметов");
                Console.WriteLine("4. Бой: Игрок vs Гоблин (Критический удар)");
                Console.WriteLine("5. Бой: Игрок vs Скелет (Игнорирование брони)");
                Console.WriteLine("6. Бой: Игрок vs Маг (Шанс заморозки)");
                Console.WriteLine("7. RNG Цикл: 5 случайных боев с разными сущностями");
                Console.WriteLine("8. Выход");
                Console.Write("\nВыберите действие (1-8): ");

                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Неверный ввод. Нажмите Enter и попробуйте снова.");
                    Console.ReadLine();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        RunOriginalRngTests();
                        Console.WriteLine("\nНажмите Enter для возврата в меню");
                        Console.ReadLine();
                        break;
                    case 2:
                        RunLootTests();
                        Console.WriteLine("\nНажмите Enter для возврата в меню");
                        Console.ReadLine();
                        break;
                    case 3:
                        RunRarityColorTests();
                        Console.WriteLine("\nНажмите Enter для возврата в меню");
                        Console.ReadLine();
                        break;
                    case 4:
                        RunBattleTest(
                            new Player("Герой", 100, 100, 15, 5),
                            new Goblin("Гоблин-разбойник", 60, 60, 12, 2)
                        );
                        Console.ReadLine();
                        break;
                    case 5:
                        RunBattleTest(
                            new Player("Герой", 100, 100, 15, 8),
                            new Skeleton("Скелет-воин", 80, 80, 18, 4)
                        );
                        Console.ReadLine();
                        break;
                    case 6:
                        RunBattleTest(
                            new Player("Герой", 100, 100, 15, 5),
                            new Mage("Маг-испытатель", 70, 70, 20, 3)
                        );
                        Console.ReadLine();
                        break;
                    case 7:
                        RunRngLoopTest();
                        Console.WriteLine("\nНажмите Enter для возврата в меню");
                        Console.ReadLine();
                        break;
                    case 8:
                        exit = true;
                        Console.WriteLine("Выход из программы.");
                        break;
                    default:
                        Console.WriteLine("Неверный номер. Выберите от 1 до 8.");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
