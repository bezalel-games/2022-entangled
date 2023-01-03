using Cards.Buffs.ActiveBuffs;
using Cards.CardElementClasses;
using Cards.Factory;
using Player;

namespace Cards.Buffs.PassiveBuffs
{
    public class ExpandExplosion : Buff
    {
        #region Fields

        private readonly float _growthIncrease;

        #endregion

        #region Buff Implementation

        public override void Apply(PlayerController playerController)
        {
            playerController.Yoyo.ExplosiveYoyo.ExplosionRadius *= _growthIncrease;
        }

        public override BuffType Type => BuffType.EXPAND_EXPLOSION;

        #endregion

        #region Constructor

        public ExpandExplosion(CardElementClassAttributes attributes, Rarity rarity, float growthIncrease)
            : base(attributes, rarity)
        {
            _growthIncrease = growthIncrease;
        }

        #endregion
        
    }
}