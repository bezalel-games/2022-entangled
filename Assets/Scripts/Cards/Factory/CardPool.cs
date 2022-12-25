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
        private readonly List<(BuffType buff, Rarities rarities)> _buffPool;
        private readonly List<(DebuffType debuff, Rarities rarities)> _debuffPool;
        private readonly bool[] _containedBuffs;
        private readonly bool[] _containedDebuffs;

        private int[] _raritiesBuffer = new int[NUMBER_OF_RARITIES];

        #endregion
        
        #region Events

        public event Action PoolUpdated;
        
        #endregion

        #region Constructor

        public CardPool(params int[] weights)
        {
            if (weights.Length != NUMBER_OF_RARITIES)
                throw new ArgumentException("length of weights should be exactly 3");
            _weights = weights;

            var numOfBuffs = Enum.GetNames(typeof(BuffType)).Length;
            _buffPool = new(numOfBuffs);
            _containedBuffs = new bool[numOfBuffs];

            var numOfDebuffs = Enum.GetNames(typeof(DebuffType)).Length;
            _debuffPool = new(numOfDebuffs);
            _containedDebuffs = new bool[numOfDebuffs];
        }

        #endregion

        #region Public Methods

        public void Add(BuffType buff, Rarities rarities)
        {
            if (_containedBuffs[(int)buff]) return;
            _buffPool.Add((buff, rarities));
            _containedBuffs[(int)buff] = true;
        }

        public void Add(DebuffType debuff, Rarities rarities)
        {
            if (_containedDebuffs[(int)debuff]) return;
            _debuffPool.Add((debuff, rarities));
            _containedDebuffs[(int)debuff] = true;
        }

        public bool Contains(BuffType buff) => _containedBuffs[(int)buff];
        public bool Contains(DebuffType debuff) => _containedDebuffs[(int)debuff];

        public void Remove(BuffType buff)
        {
            if (!_containedBuffs[(int)buff]) return;
            _buffPool.RemoveAll(element => element.buff == buff);
        }

        public void Remove(DebuffType debuff)
        {
            if (!_containedDebuffs[(int)debuff]) return;
            _debuffPool.RemoveAll(element => element.debuff == debuff);
        }

        public (BuffType type, Rarity rarity) GetRandomBuff() => GetRandom(_buffPool);

        public (DebuffType type, Rarity rarity) GetRandomDebuff() => GetRandom(_debuffPool);

        #endregion

        #region Public Methods

        private (T, Rarity) GetRandom<T>(IReadOnlyList<(T type, Rarities rarities)> pool) where T : Enum
        {
            var element = pool[Random.Range(0, pool.Count)];
            var toArrResult = element.rarities.ToIntArrayNonAlloc(ref _raritiesBuffer);
            if (toArrResult.onlyOneRarity)
                return (element.type, toArrResult.rarity);

            int range = 0;
            for (int i = 0; i < NUMBER_OF_RARITIES; ++i)
            {
                range += _raritiesBuffer[i] *= _weights[i];
            }

            var randomInRange = Random.Range(0, range);
            for (int i = 0; i < NUMBER_OF_RARITIES - 1; ++i)
            {
                if (randomInRange < _raritiesBuffer[i])
                    return (element.type, (Rarity)i);
            }

            return (element.type, Rarity.EPIC);
        }

        #endregion
    }
}