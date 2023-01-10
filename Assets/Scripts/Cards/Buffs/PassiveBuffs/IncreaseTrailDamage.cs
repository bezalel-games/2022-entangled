using Cards.CardElementClasses;
using Cards.Factory;
using Player;

namespace Cards.Buffs.PassiveBuffs
{
    public class IncreaseTrailDamage : Buff
    {
        #region Fields

        private readonly float _damageIncrease;

        #endregion

        #region Buff Implementation

        public override void Apply(PlayerController playerController)
        {
            playerController.Yoyo.LinePrefab.Damage *= _damageIncrease;
        }

        public override BuffType Type => BuffType.INCREASE_TRAIL_DAMAGE;

        #endregion

        #region Constructor

        public IncreaseTrailDamage(CardElementClassAttributes attributes, Rarity rarity, float damageIncrease)
            : base(attributes, rarity)
        {
            _damageIncrease = damageIncrease;
        }

        #endregion
        
    }
}