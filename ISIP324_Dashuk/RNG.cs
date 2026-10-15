using System;
using System.Collections.Generic;

namespace Roguelike
{
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
}