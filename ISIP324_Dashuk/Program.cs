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

    internal class Program
    {
        // Метод для пошагового боя 
        static bool RunInteractiveBattle(Player player, Entity enemy)
        {
            Console.WriteLine($" НАЧАЛО БОЯ: {player.Name} vs {enemy.Name}");
            int round = 1;

            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine($"\nРаунд {round}");
                Console.WriteLine($"[Статус] Ваше HP: {player.Current_HP:F1}/{player.Max_HP:F1} | Враг HP: {enemy.Current_HP:F1}/{enemy.Max_HP:F1}");

                // Проверка заморозки игрока
                if (player.IsFrosen)
                {
                    Console.WriteLine($"\x1b[36mВы заморожены и пропускаете свой ход!\x1b[0m");
                    player.IsFrosen = false;
                }
                else
                {
                    // Ход игрока
                    Console.WriteLine("Выберите действие:");
                    Console.WriteLine("1. Атаковать");
                    Console.WriteLine("2. Защищаться");
                    string actionInput = Console.ReadLine();

                    if (int.TryParse(actionInput, out int actionChoice) && actionChoice == 2)
                    {
                        player.playerAction = PlayerAction.To_Defend;
                        Console.WriteLine("Вы заняли оборонительную позицию!");
                    }
                    else
                    {
                        player.playerAction = PlayerAction.To_Attack;
                        player.PerformAttack(enemy);
                    }
                }

                if (!enemy.IsAlive)
                {
                    Console.WriteLine($"\n{enemy.Name} повержен в бою!");
                    break;
                }

                // Ход врага
                if (enemy.IsFrosen)
                {
                    Console.WriteLine($"\x1b[36m{enemy.Name} заморожен и пропускает ход!\x1b[0m");
                    enemy.IsFrosen = false;
                }
                else
                {
                    // Если враг атакует, а игрок выбрал защиту
                    if (player.playerAction == PlayerAction.To_Defend)
                    {
                        // 40% шанс полностью уклониться
                        if (RNG.Instance.RollChance(40))
                        {
                            Console.WriteLine($"\n[Защита] {player.Name} ловко уклонился от атаки {enemy.Name}!");
                        }
                        else
                        {
                            // Срабатывает блок: уменьшение урона на 70-100% от характеристики защиты
                            double blockPercent = RNG.Instance.RandomRange(70, 100) / 100.0;
                            double mitigatedDamage = player.GetTotalProtection() * blockPercent;

                            Console.WriteLine($"\n[Защита] Уклонение не удалось, но сработал блок (-{mitigatedDamage:F1} урона за счет защиты)!");

                            // эмулирование блока:
                            double rawDmg = enemy.GetTotalAttack();
                            // Вызываем атаку врага с учетом уменьшения через блок
                            double effectiveRaw = Math.Max(1, rawDmg - mitigatedDamage);
                            // Пусть враг нанесет урон без повторного вычета брони (ignoreArmor = true, так как блок уже учел броню)
                            enemy.PerformAttack(player); 
                        }
                    }
                    else
                    {
                        enemy.PerformAttack(player);
                    }

                    if (!player.IsAlive)
                    {
                        Console.WriteLine($"\n{player.Name} погиб в бою... Игра окончена.");
                        break;
                    }
                }

                round++;
            }

            return player.IsAlive;
        }

        // Метод для обработки сундука 
        static void HandleChest(Player player)
        {
            Console.WriteLine($" ВЫ НАШЛИ СУНДУК С СОКРОВИЩЕМ! ");

            Item loot = LootManager.Instance.GenerateChestLoot((int)player.Max_HP);
            Console.WriteLine($"Из сундука выпал предмет: {loot}");
            Console.WriteLine($"Характеристики: {loot.GetStatsDescription()}");

            if (loot is Potion potion)
            {
                player.Heal(potion.HPRecovery);
            }
            else if (loot is Weapon newWeapon)
            {
                Console.WriteLine("\nВаше текущее оружие:");
                if (player.EquippedWeapon != null)
                    Console.WriteLine($"- {player.EquippedWeapon} | {player.EquippedWeapon.GetStatsDescription()}");
                else
                    Console.WriteLine("- Отсутствует (кулаки)");

                Console.WriteLine("\nХотите заменить текущее оружие новым? (1 - Да, взять, 2 - Нет, выбросить)");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    player.EquipWeapon(newWeapon);
                }
                else
                {
                    Console.WriteLine("Вы выбросили предмет.");
                }
            }
            else if (loot is Armor newArmor)
            {
                Console.WriteLine("\nВаша текущая броня:");
                if (player.EquippedArmor != null)
                    Console.WriteLine($"- {player.EquippedArmor} | {player.EquippedArmor.GetStatsDescription()}");
                else
                    Console.WriteLine("- Отсутствует");

                Console.WriteLine("\nХотите надеть новую броню? (1 - Да, взять, 2 - Нет, выбросить)");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    player.EquipArmor(newArmor);
                }
                else
                {
                    Console.WriteLine("Вы выбросили предмет.");
                }
            }
        }

        // Основной игровой процесс-рогалик
        static void RunRoguelikeGame()
        {
            Console.Clear();
            Console.WriteLine("Введите имя вашего героя:");
            string heroName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(heroName)) heroName = "Герой";

            Player player = new Player(heroName, 100, 100, 15, 5);
            int turnCount = 1;

            Console.WriteLine($"\nПриключение начинается! Добро пожаловать, {player.Name}!");
            Console.WriteLine("Нажимайте Enter для продолжения каждого хода.");
            Console.ReadLine();

            while (player.IsAlive)
            {
                Console.Clear();
                Console.WriteLine($"ХОД #{turnCount}");
                Console.WriteLine($"Герой: {player.Name} | HP: {player.Current_HP}/{player.Max_HP} | Атака: {player.GetTotalAttack()} | Защита: {player.GetTotalProtection()}");
                Console.WriteLine(new string('-', 40));

                // Каждые 10 ходов - босс
                if (turnCount % 10 == 0)
                {
                    Console.WriteLine("\n\x1b[31mВНИМАНИЕ! На горизонте появляется БОСС!\x1b[0m");

                    Entity boss;
                    switch (turnCount)
                    {
                        case 10:
                            boss = new VVG(100, 100, 15, 5, 35);
                            break;
                        case 20:
                            boss = new Kowalski(100, 100, 18, 6);
                            break;
                        case 30:
                            boss = new Pestov(100, 100, 20, 5);
                            break;
                        default:
                            boss = new Archmage("Архимаг C++", 100, 100, 22, 6, 25);
                            break;
                    }

                    bool survived = RunInteractiveBattle(player, boss);
                    if (!survived) break;
                }
                else
                {
                    // 50 на 50: сундук или враг
                    bool isChest = RNG.Instance.RollChance(50);

                    if (isChest)
                    {
                        HandleChest(player);
                    }
                    else
                    {
                        // Случайный выбор типа врага
                        var enemyTypes = new List<string> { "Goblin", "Skeleton", "Mage" };
                        string chosenType = RNG.Instance.RandomChoice(enemyTypes);

                        Entity enemy;
                        switch (chosenType)
                        {
                            case "Goblin":
                                enemy = new Goblin("Дикий гоблин", 60, 60, 12, 2, 35);
                                break;
                            case "Skeleton":
                                enemy = new Skeleton("Блуждающий скелет", 70, 70, 16, 4);
                                break;
                            default:
                                enemy = new Mage("Бродячий маг", 50, 50, 18, 2, 20);
                                break;
                        }

                        Console.WriteLine($"\nНа вас напал враг: {enemy.Name}!");
                        bool survived = RunInteractiveBattle(player, enemy);
                        if (!survived) break;
                    }
                }

                Console.WriteLine("\nНажмите Enter для перехода к следующему ходу...");
                Console.ReadLine();
                turnCount++;
            }

            Console.WriteLine($"\nИгра окончена! Вы продержались {turnCount} ходов.");
            Console.WriteLine("Нажмите Enter для возврата в главное меню.");
            Console.ReadLine();
        }

        //  тестовые методы
        static void RunBattleTest(Player player, Entity enemy)
        {
            RunInteractiveBattle(player, enemy);
            Console.WriteLine("\nНажмите Enter для возврата в меню");
            Console.ReadLine();
        }

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

                RunInteractiveBattle(player, randomEnemy);
                Console.WriteLine(new string('-', 40));
            }
            Console.WriteLine("\nRNG ЦИКЛ ЗАВЕРШЕН");
            Console.ReadLine();
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

        static void RunBossTest_VVG()
        {
            Console.WriteLine("\nТЕСТ БОССА: VVG (Гоблин-Босс)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new VVG(120, 120, 15, 5, 40);
            RunBattleTest(player, boss);
        }

        static void RunBossTest_Kowalski()
        {
            Console.WriteLine("\nТЕСТ БОССА: Kowalski (Скелет-Босс)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new Kowalski(150, 150, 20, 8);
            RunBattleTest(player, boss);
        }

        static void RunBossTest_Pestov()
        {
            Console.WriteLine("\nТЕСТ БОССА: Pestov (Скелет-Босс с заморозкой)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new Pestov(100, 100, 25, 6);
            RunBattleTest(player, boss);
        }

        static void RunBossTest_Archmage()
        {
            Console.WriteLine("\nТЕСТ БОССА: Archmage_C (Маг-Босс)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new Archmage("Archmage_C", 100, 100, 25, 5, 30);
            RunBattleTest(player, boss);
        }

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("КОНСОЛЬНЫЙ РОГАЛИК");
                Console.WriteLine("0. Запустить игру (Основной режим пошаговой игры)");
                Console.WriteLine("1. RNG (Шансы, Диапазоны, Выбор)");
                Console.WriteLine("2. LootManager (50 сундуков)");
                Console.WriteLine("3. Редкость и цвета предметов");
                Console.WriteLine("4. Бой: Игрок vs Гоблин (Критический удар)");
                Console.WriteLine("5. Бой: Игрок vs Скелет (Игнорирование брони)");
                Console.WriteLine("6. Бой: Игрок vs Маг (Шанс заморозки)");
                Console.WriteLine("7. RNG Цикл: 5 случайных боев с разными сущностями");
                Console.WriteLine("8. Бой: Игрок vs VVG (Босс Гоблин)");
                Console.WriteLine("9. Бой: Игрок vs Kowalski (Босс Скелет)");
                Console.WriteLine("10. Бой: Игрок vs Pestov (Босс Скелет с заморозкой)");
                Console.WriteLine("11. Бой: Игрок vs Archmage_C (Босс Маг)");
                Console.WriteLine("12. Выход");

                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Неверный ввод. Нажмите Enter и попробуйте снова.");
                    Console.ReadLine();
                    continue;
                }

                switch (choice)
                {
                    case 0:
                        RunRoguelikeGame();
                        break;
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
                        break;
                    case 5:
                        RunBattleTest(
                            new Player("Герой", 100, 100, 15, 8),
                            new Skeleton("Скелет-воин", 80, 80, 18, 4)
                        );
                        break;
                    case 6:
                        RunBattleTest(
                            new Player("Герой", 100, 100, 15, 5),
                            new Mage("Маг-испытатель", 70, 70, 20, 3)
                        );
                        break;
                    case 7:
                        RunRngLoopTest();
                        break;
                    case 8:
                        RunBossTest_VVG();
                        break;
                    case 9:
                        RunBossTest_Kowalski();
                        break;
                    case 10:
                        RunBossTest_Pestov();
                        break;
                    case 11:
                        RunBossTest_Archmage();
                        break;
                    case 12:
                        exit = true;
                        Console.WriteLine("Выход из программы.");
                        break;
                    default:
                        Console.WriteLine("Неверный номер. Выберите от 0 до 12.");
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
