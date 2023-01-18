using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;

namespace Cards.Debuffs
{
    public class FasterEnemies : EnemyDebuff
    {
        #region Fields

        private readonly int _enemyType;
        private readonly float _speedMultiplier;

        #endregion

        #region Debuff Implementation

        public override void Apply(EnemyDictionary enemyDictionary)
        {
            enemyDictionary[_enemyType].MaxSpeed *= _speedMultiplier;
        }

        public override DebuffType Type => DebuffType.FASTER_GOOMBAS + _enemyType;

        #endregion

        #region Constructor

        public FasterEnemies(CardElementClassAttributes attributes, Rarity rarity, int enemyType, float speedMultiplier)
            : base(attributes, rarity)
        {
            _enemyType = enemyType;
            _speedMultiplier = speedMultiplier;
        }

        #endregion
    }
}