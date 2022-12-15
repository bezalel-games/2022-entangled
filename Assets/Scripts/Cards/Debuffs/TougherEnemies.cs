using System;
using Enemies;

namespace Cards.Debuffs
{
    class TougherEnemies : IDebuff
    {
        #region Fields

        private readonly int _enemyType;
        private readonly int _hpAddition;

        #endregion

        #region ICardProperty Implementation

        public string Name => "More Enemies";
        public string Description => "More enemies will spawn in the following rooms";
        public string Rarity => "Normal";

        #endregion

        #region IDebuff Implementation

        public void Apply(EnemyDictionary enemyDictionary)
        {
            enemyDictionary[_enemyType].MaxHp += _hpAddition;
        }

        #endregion

        #region Constructor

        public TougherEnemies(int enemyType, int hpAddition)
        {
            _enemyType = enemyType;
            _hpAddition = hpAddition;
        }

        #endregion
    }
}