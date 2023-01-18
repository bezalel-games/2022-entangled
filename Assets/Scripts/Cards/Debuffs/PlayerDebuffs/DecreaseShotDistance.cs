using Cards.CardElementClasses;
using Cards.Factory;
using Enemies;
using Managers;
using Player;

namespace Cards.Debuffs
{
    public class DecreaseShotDistance : PlayerDebuff
    {
        #region Fields
        
        private readonly float _distanceMultiplier;

        #endregion

        #region Debuff Implementation

        public override void Apply(PlayerController player)
        {
            player.Yoyo.MaxDistance *= _distanceMultiplier;
        }

        public override DebuffType Type => DebuffType.DECREASE_SHOT_DISTANCE;

        #endregion

        #region Constructor

        public DecreaseShotDistance(CardElementClassAttributes attributes, Rarity rarity, float decreaseMultiplier)
            : base(attributes, rarity)
        {
            _distanceMultiplier = decreaseMultiplier;
        }

        #endregion
    }
}