using System;
using Enemies;

namespace Cards.Debuffs
{
    class TougherEnemies : IDebuff
    {
        #region Fields

        private readonly int _enemyType;
        private readonly float _hpAddition;

        #endregion

        #region ICardProperty Implementation

        public string Name => "Tougher Enemies";
        public string Description => "Enemies will have more HP";
        public string Rarity => "Normal";

        #endregion

        #region IDebuff Implementation

        public void Apply(EnemyDictionary enemyDictionary)
        {
            enemyDictionary[_enemyType].MaxHp += _hpAddition;
        }

        #endregion

        #region Constructor

        public TougherEnemies(int enemyType, float hpAddition)
        {
            _enemyType = enemyType;
            _hpAddition = hpAddition;
        }

        #endregion
    }
}