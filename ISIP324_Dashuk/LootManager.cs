using System;
using System.Collections.Generic;

namespace Roguelike
{
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
}