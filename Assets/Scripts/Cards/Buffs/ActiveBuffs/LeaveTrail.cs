using Cards.CardElementClasses;
using Cards.Factory;
using Player;
using Unity.VisualScripting;

namespace Cards.Buffs.ActiveBuffs
{
    public class LeaveTrail : Buff
    {

        #region Fields

        private float _initStayTime;
        private float _initDamage;

        #endregion
        
        #region Public Methods

        public LeaveTrail(CardElementClassAttributes attributes, Rarity rarity, float initDamage, float initStayTime) 
            : base(attributes, rarity)
        {
            _initDamage = initDamage;
            _initStayTime = initStayTime;
        }

        public override void Apply(PlayerController playerController)
        {
            var yoyo = playerController.Yoyo;

            yoyo.FinishedPrecision += yoyo.LeaveTrail;
            yoyo.LinePrefab.Damage = _initDamage * yoyo.Damage;
            yoyo.LinePrefab.StayTime = _initStayTime;
        }

        public override BuffType Type => BuffType.LEAVE_TRAIL;

        #endregion
    }
}