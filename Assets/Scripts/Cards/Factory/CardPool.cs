using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Cards.Factory
{
    public class CardPool
    {
        #region Fields

        private const int NUMBER_OF_RARITIES = 3;

        private readonly int[] _weights;
        private readonly List<BuffType>[] _buffPools = new List<BuffType>[NUMBER_OF_RARITIES];
        private readonly List<DebuffType>[] _debuffPools = new List<DebuffType>[NUMBER_OF_RARITIES];

        #endregion

        #region Constructor

        public CardPool(int[] weights)
        {
            if (weights.Length != NUMBER_OF_RARITIES)
                throw new ArgumentException("length of weights should be exactly 3");
            _weights = weights;
            for (int i = 0; i < NUMBER_OF_RARITIES; ++i)
            {
                _buffPools[i] = new List<BuffType>();
                _debuffPools[i] = new List<DebuffType>();
            }
        }

        #endregion

        #region Public Methods

        public void Add(BuffType buff, Rarity rarity)
        {
            var pool = _buffPools[(int)rarity];
            if (pool.Contains(buff)) return;
            pool.Add(buff);
        }

        public void Add(DebuffType debuff, Rarity rarity)
        {
            var pool = _debuffPools[(int)rarity];
            if (pool.Contains(debuff)) return;
            pool.Add(debuff);
        }

        public void Remove(BuffType buff, Rarity rarity)
        {
            _buffPools[(int)rarity].Remove(buff);
        }

        public void Remove(DebuffType debuff, Rarity rarity)
        {
            _debuffPools[(int)rarity].Remove(debuff);
        }

        public (BuffType type, Rarity rarity) GetRandomBuff()
        {
            int range = 0;
            for (int i = 0; i < NUMBER_OF_RARITIES; ++i)
                range += _buffPools[i].Count * _weights[i];
            var index = Random.Range(0, range);

            for (int i = 0; i < NUMBER_OF_RARITIES; ++i)
            {
                if (index < _buffPools[i].Count * _weights[i])
                    return (_buffPools[i][index / _weights[i]], (Rarity)i);
                index -= _buffPools[i].Count * _weights[i];
            }

            throw new Exception("Algorithm is wrong");
        }

        public (DebuffType type, Rarity rarity) GetRandomDebuff()
        {
            int range = 0;
            for (int i = 0; i < NUMBER_OF_RARITIES; ++i)
                range += _debuffPools[i].Count * _weights[i];
            var index = Random.Range(0, range);

            for (int i = 0; i < NUMBER_OF_RARITIES; ++i)
            {
                if (index < _debuffPools[i].Count * _weights[i])
                    return (_debuffPools[i][index / _weights[i]], (Rarity)i);
                index -= _debuffPools[i].Count * _weights[i];
            }

            throw new Exception("Algorithm is wrong");
        }

        #endregion
    }
}