using System;
using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;

namespace Cards.Debuffs
{
    public class MoreEnemies : EnemyDebuff
    {
        #region Fields

        private readonly int _enemyType;
        private readonly float _enemyRankMultiplier;

        #endregion

        #region Debuff Implementation

        public override void Apply(EnemyDictionary enemyDictionary)
        {
            var rank = enemyDictionary[_enemyType].Rank;
            enemyDictionary[_enemyType].Rank = Math.Max((int)(rank * _enemyRankMultiplier), 1);
        }
        
        public override DebuffType Type => DebuffType.MORE_GOOMBAS + _enemyType;

        #endregion

        #region Constructor

        public MoreEnemies(CardElementClassAttributes attributes, Rarity rarity, int enemyType, float enemyQuantityMultiplier)
            : base(attributes, rarity)
        {
            _enemyType = enemyType;
            _enemyRankMultiplier = 1 / enemyQuantityMultiplier;
        }

        #endregion
    }
}