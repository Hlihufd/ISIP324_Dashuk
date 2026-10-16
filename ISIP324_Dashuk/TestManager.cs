using System;
using System.Collections.Generic;

namespace Roguelike
{
    public static class TestManager
    {
        // Метод для проведения одного тестового боя
        public static void RunBattleTest(Player player, Entity enemy)
        {
            RunInteractiveBattleTest(player, enemy);
            Console.WriteLine("\nНажмите Enter для возврата в меню");
            Console.ReadLine();
        }

        //  бой дублирует логику боя
        private static bool RunInteractiveBattleTest(Player player, Entity enemy)
        {
            Console.WriteLine($" НАЧАЛО БОЯ: {player.Name} vs {enemy.Name}");
            int round = 1;

            while (player.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine($"\n--- Раунд {round} ---");
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

        // Метод для RNG-цикла тестирования
        public static void RunRngLoopTest()
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

                RunInteractiveBattleTest(player, randomEnemy);
                Console.WriteLine(new string('-', 40));
            }
            Console.WriteLine("\nRNG ЦИКЛ ЗАВЕРШЕН");
            Console.ReadLine();
        }

        public static void RunOriginalRngTests()
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

        public static void RunLootTests()
        {
            Console.WriteLine("\nТест 5: Генерация лута (50 сундуков)");
            int testPlayerMaxHP = 100;

            for (int i = 1; i <= 50; i++)
            {
                Item loot = LootManager.Instance.GenerateChestLoot(testPlayerMaxHP);
                Console.WriteLine($"Сундук {i,2}: {loot.ToString(),-35} | {loot.GetStatsDescription()}");
            }
        }

        public static void RunRarityColorTests()
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

        public static void RunBossTest_VVG()
        {
            Console.WriteLine("\nТЕСТ БОССА: VVG (Гоблин-Босс)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new VVG(120, 120, 15, 5, 40);
            RunBattleTest(player, boss);
        }

        public static void RunBossTest_Kowalski()
        {
            Console.WriteLine("\nТЕСТ БОССА: Kowalski (Скелет-Босс)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new Kowalski(150, 150, 20, 8);
            RunBattleTest(player, boss);
        }

        public static void RunBossTest_Pestov()
        {
            Console.WriteLine("\nТЕСТ БОССА: Pestov (Скелет-Босс с заморозкой)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new Pestov(100, 100, 25, 6);
            RunBattleTest(player, boss);
        }

        public static void RunBossTest_Archmage()
        {
            Console.WriteLine("\nТЕСТ БОССА: Archmage_C (Маг-Босс)");
            Player player = new Player("Герой", 150, 150, 20, 10);
            player.EquipWeapon(new Weapon("Стальной меч", 10, Rarity.Uncommon));
            player.EquipArmor(new Armor("Кожаный нагрудник", 5, Rarity.Common));

            Entity boss = new Archmage("Archmage_C", 100, 100, 25, 5, 30);
            RunBattleTest(player, boss);
        }
    }
}
