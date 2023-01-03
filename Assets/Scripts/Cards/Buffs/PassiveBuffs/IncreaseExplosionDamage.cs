using Cards.Buffs.ActiveBuffs;
using Cards.CardElementClasses;
using Cards.Factory;
using Player;

namespace Cards.Buffs.PassiveBuffs
{
    public class IncreaseExplosionDamage : Buff
    {
        #region Fields

        private readonly float _damageIncrease;

        #endregion

        #region Buff Implementation

        public override void Apply(PlayerController playerController)
        {
            playerController.Yoyo.ExplosiveYoyo.Damage *= _damageIncrease;
        }

        public override BuffType Type => BuffType.INCREASE_EXPLOSION_DAMAGE;

        #endregion

        #region Constructor

        public IncreaseExplosionDamage(CardElementClassAttributes attributes, Rarity rarity, float damageIncrease)
            : base(attributes, rarity)
        {
            _damageIncrease = damageIncrease;
        }

        #endregion
        
    }
}