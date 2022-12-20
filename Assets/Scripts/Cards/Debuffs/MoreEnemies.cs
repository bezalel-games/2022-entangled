using System;
using Cards.CardElementClasses;
using Enemies;

namespace Cards.Debuffs
{
    class MoreEnemies : Debuff
    {
        #region Fields

        private readonly int _enemyType;
        private readonly int _rankDemotion;

        #endregion

        #region Debuff Implementation

        public override void Apply(EnemyDictionary enemyDictionary)
        {
            var rank = enemyDictionary[_enemyType].Rank;
            enemyDictionary[_enemyType].Rank = Math.Max(rank - _rankDemotion, 1);
        }

        #endregion

        #region Constructor

        public MoreEnemies(CardElementClassAttributes attributes, Rarity rarity, int enemyType, int rankDemotion)
            : base(attributes, rarity)
        {
            _enemyType = enemyType;
            _rankDemotion = rankDemotion;
        }

        #endregion
    }
}