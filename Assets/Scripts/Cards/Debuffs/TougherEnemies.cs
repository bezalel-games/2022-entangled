using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;

namespace Cards.Debuffs
{
    public class TougherEnemies : Debuff
    {
        #region Fields

        private readonly int _enemyType;
        private readonly float _hpMultiplier;

        #endregion

        #region Debuff Implementation

        public override void Apply(EnemyDictionary enemyDictionary)
        {
            enemyDictionary[_enemyType].MaxHp *= _hpMultiplier;
        }

        public override DebuffType Type => DebuffType.TOUGHER_GOOMBAS + _enemyType;

        #endregion

        #region Constructor

        public TougherEnemies(CardElementClassAttributes attributes, Rarity rarity, int enemyType, float hpMultiplier)
            : base(attributes, rarity)
        {
            _enemyType = enemyType;
            _hpMultiplier = hpMultiplier;
        }

        #endregion
    }
}