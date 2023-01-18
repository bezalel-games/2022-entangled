using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;
using Managers;
using Player;

namespace Cards.Debuffs
{
    public class DecreaseMpRegeneration : PlayerDebuff
    {
        #region Fields
        
        private readonly float _regenMultiplier;

        #endregion

        #region Debuff Implementation

        public override void Apply(PlayerController player)
        {
            player.MpRecovery *= _regenMultiplier;
        }

        public override DebuffType Type => DebuffType.DECREASE_MP_REGEN;

        #endregion

        #region Constructor

        public DecreaseMpRegeneration(CardElementClassAttributes attributes, Rarity rarity, float decreaseMultiplier)
            : base(attributes, rarity)
        {
            _regenMultiplier = decreaseMultiplier;
        }

        #endregion
    }
}