using System;
using System.Collections.Generic;

namespace Roguelike
{
    internal class Program
    {
        static bool RunInteractiveBattle(Player player, Entity enemy)
        {
            Console.WriteLine($" НАЧАЛО БОЯ: {player.Name} vs {enemy.Name}");
            int round = 1;

            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine($"\nРаунд {round}");
                Console.WriteLine($"[Статус] Ваше HP: {player.Current_HP:F1}/{player.Max_HP:F1} | Враг HP: {enemy.Current_HP:F1}/{enemy.Max_HP:F1}");

                if (player.IsFrosen)
                {
                    Console.WriteLine($"\x1b[36mВы заморожены и пропускаете свой ход!\x1b[0m");
                    player.IsFrosen = false;
                }
                else
                {
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

                if (enemy.IsFrosen)
                {
                    Console.WriteLine($"\x1b[36m{enemy.Name} заморожен и пропускает ход!\x1b[0m");
                    enemy.IsFrosen = false;
                }
                else
                {
                    if (player.playerAction == PlayerAction.To_Defend)
                    {
                        if (RNG.Instance.RollChance(40))
                        {
                            Console.WriteLine($"\n[Защита] {player.Name} ловко уклонился от атаки {enemy.Name}!");
                        }
                        else
                        {
                            double blockPercent = RNG.Instance.RandomRange(70, 100) / 100.0;
                            double mitigatedDamage = player.GetTotalProtection() * blockPercent;
                            Console.WriteLine($"\n[Защита] Уклонение не удалось, но сработал блок (-{mitigatedDamage:F1} урона за счет защиты)!");
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
                if (input == "1") player.EquipWeapon(newWeapon);
                else Console.WriteLine("Вы выбросили предмет.");
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
                if (input == "1") player.EquipArmor(newArmor);
                else Console.WriteLine("Вы выбросили предмет.");
            }
        }

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
                Console.WriteLine($"#{turnCount}");
                Console.WriteLine($"Герой: {player.Name} | HP: {player.Current_HP}/{player.Max_HP} | Атака: {player.GetTotalAttack()} | Защита: {player.GetTotalProtection()}");
                Console.WriteLine(new string('-', 40));

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
                    bool isChest = RNG.Instance.RollChance(50);

                    if (isChest)
                    {
                        HandleChest(player);
                    }
                    else
                    {
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
                        TestManager.RunOriginalRngTests();
                        Console.WriteLine("\nНажмите Enter для возврата в меню");
                        Console.ReadLine();
                        break;
                    case 2:
                        TestManager.RunLootTests();
                        Console.WriteLine("\nНажмите Enter для возврата в меню");
                        Console.ReadLine();
                        break;
                    case 3:
                        TestManager.RunRarityColorTests();
                        Console.WriteLine("\nНажмите Enter для возврата в меню");
                        Console.ReadLine();
                        break;
                    case 4:
                        TestManager.RunBattleTest(
                            new Player("Герой", 100, 100, 15, 5),
                            new Goblin("Гоблин-разбойник", 60, 60, 12, 2)
                        );
                        break;
                    case 5:
                        TestManager.RunBattleTest(
                            new Player("Герой", 100, 100, 15, 8),
                            new Skeleton("Скелет-воин", 80, 80, 18, 4)
                        );
                        break;
                    case 6:
                        TestManager.RunBattleTest(
                            new Player("Герой", 100, 100, 15, 5),
                            new Mage("Маг-испытатель", 70, 70, 20, 3)
                        );
                        break;
                    case 7:
                        TestManager.RunRngLoopTest();
                        break;
                    case 8:
                        TestManager.RunBossTest_VVG();
                        break;
                    case 9:
                        TestManager.RunBossTest_Kowalski();
                        break;
                    case 10:
                        TestManager.RunBossTest_Pestov();
                        break;
                    case 11:
                        TestManager.RunBossTest_Archmage();
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