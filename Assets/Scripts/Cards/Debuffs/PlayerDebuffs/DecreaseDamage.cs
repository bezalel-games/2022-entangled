using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;
using Managers;
using Player;

namespace Cards.Debuffs
{
    public class DecreaseDamage : PlayerDebuff
    {
        #region Fields
        
        private readonly float _damageMultiplier;

        #endregion

        #region Debuff Implementation

        public override void Apply(PlayerController player)
        {
            player.Yoyo.Damage *= _damageMultiplier;
        }

        public override DebuffType Type => DebuffType.DECREASE_DAMAGE;

        #endregion

        #region Constructor

        public DecreaseDamage(CardElementClassAttributes attributes, Rarity rarity, float decreaseMultiplier)
            : base(attributes, rarity)
        {
            _damageMultiplier = decreaseMultiplier;
        }

        #endregion
    }
}