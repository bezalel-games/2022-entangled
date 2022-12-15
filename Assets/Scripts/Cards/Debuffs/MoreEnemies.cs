using System;
using Enemies;

namespace Cards.Debuffs
{
    class MoreEnemies : IDebuff
    {
        #region Fields

        private readonly int _enemyType;
        private readonly int _rankDemotion;

        #endregion

        #region ICardProperty Implementation

        public string Name => "More Enemies";
        public string Description => "More enemies will spawn in the following rooms";
        public string Rarity => "Normal";

        #endregion

        #region IDebuff Implementation

        public void Apply(EnemyDictionary enemyDictionary)
        {
            var rank = enemyDictionary[_enemyType].Rank;
            enemyDictionary[_enemyType].Rank = Math.Max(rank - _rankDemotion, 1);
        }

        #endregion

        #region Constructor

        public MoreEnemies(int enemyType, int rankDemotion)
        {
            _enemyType = enemyType;
            _rankDemotion = rankDemotion;
        }

        #endregion
    }
}