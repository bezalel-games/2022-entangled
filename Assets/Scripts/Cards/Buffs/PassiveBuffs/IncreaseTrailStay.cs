using Cards.CardElementClasses;
using Cards.Factory;
using Player;

namespace Cards.Buffs.PassiveBuffs
{
    public class IncreaseTrailStay : Buff
    {
        #region Fields

        private readonly float _stayIncrease;

        #endregion

        #region Buff Implementation

        public override void Apply(PlayerController playerController)
        {
            playerController.Yoyo.LinePrefab.StayTime *= _stayIncrease;
        }

        public override BuffType Type => BuffType.INCREASE_TRAIL_STAY;

        #endregion

        #region Constructor

        public IncreaseTrailStay(CardElementClassAttributes attributes, Rarity rarity, float stayIncrease)
            : base(attributes, rarity)
        {
            _stayIncrease = stayIncrease;
        }

        #endregion
        
    }
}