using System;
using System.Collections.Generic;
using Cards.Buffs.ActiveBuffs;
using Random = UnityEngine.Random;

namespace Cards.Factory
{
    public class CardPool
    {
        #region Fields

        private const int NUMBER_OF_RARITIES = 3;

        private readonly int[] _weights;
        private readonly CardElementPool<BuffType> _buffPool = new();
        private readonly CardElementPool<DebuffType> _debuffPool = new();

        #endregion

        #region Events

        public event Action PoolUpdated;

        #endregion

        #region Constructor

        public CardPool(int commonWeight, int rareWeight, int epicWeight)
        {
            _weights = new[] { commonWeight, rareWeight, epicWeight };
        }

        #endregion

        #region Public Methods

        public void Add(BuffType buff, Rarities rarities) => _buffPool.Add(buff, rarities);
        public void Add(DebuffType debuff, Rarities rarities) => _debuffPool.Add(debuff, rarities);

        public bool Contains(BuffType buff) => _buffPool.ContainedElements[(int)buff];
        public bool Contains(DebuffType debuff) => _debuffPool.ContainedElements[(int)debuff];

        public void Remove(BuffType buff) => _buffPool.Remove(buff);
        public void Remove(DebuffType debuff) => _debuffPool.Remove(debuff);

        public (BuffType type, Rarity rarity) GetRandomBuff() => _buffPool.GetRandom(_weights);
        public (DebuffType type, Rarity rarity) GetRandomDebuff() => _debuffPool.GetRandom(_weights);

        public void FinishedUpdating() => PoolUpdated?.Invoke();

        #endregion

        #region Classes

        private class CardElementPool<T> where T : Enum
        {
            private int[] _raritiesBuffer = new int[NUMBER_OF_RARITIES];
            private readonly List<(T type, Rarities rarities)> _pool;
            public bool[] ContainedElements { get; private set; }

            public CardElementPool()
            {
                var numOfBuffs = Enum.GetNames(typeof(T)).Length;
                _pool = new(numOfBuffs);
                ContainedElements = new bool[numOfBuffs];
            }

            public void Add(T type, Rarities rarities)
            {
                if (ContainedElements[type.IntValue()]) return;
                _pool.Add((type, rarities));
                ContainedElements[type.IntValue()] = true;
            }

            public void Remove(T type)
            {
                if (!ContainedElements[type.IntValue()]) return;
                _pool.RemoveAll(element => element.type.Equals(type));
            }

            public (T, Rarity) GetRandom(int[] weights)
            {
                var element = _pool[Random.Range(0, _pool.Count)];
                var toArrResult = element.rarities.ToIntArrayNonAlloc(ref _raritiesBuffer);
                if (toArrResult.onlyOneRarity)
                    return (element.type, toArrResult.rarity);

                int range = 0;
                for (int i = 0; i < NUMBER_OF_RARITIES; ++i)
                {
                    range += _raritiesBuffer[i] *= weights[i];
                }

                var randomInRange = Random.Range(0, range);
                for (int i = 0; i < NUMBER_OF_RARITIES - 1; ++i)
                {
                    if (randomInRange < _raritiesBuffer[i])
                        return (element.type, (Rarity)i);
                }

                return (element.type, Rarity.EPIC);
            }
        }

        #endregion
    }

    public static class EnumGenericExt
    {
        public static int IntValue<T>(this T value) where T : Enum => (int)(object)value;
    }
}