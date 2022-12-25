using Cards.CardElementClasses;
using Cards.Factory;
using Player;

namespace Cards.Buffs.ActiveBuffs
{
    public class SwapPositionWithYoyo : Buff
    {
        private float _staminaCost;

        #region Fields

        private PlayerController _playerController;
        private Yoyo _yoyo;

        #endregion

        #region Buff Implementation

        public override void Apply(PlayerController playerController)
        {
            playerController.DashStartEvent += SwapPositions;
            _playerController = playerController;
            _yoyo = playerController.Yoyo;
        }
        
        public override BuffType Type => BuffType.SWAP_POSITIONS_WITH_YOYO;

        #endregion

        #region Constructor

        public SwapPositionWithYoyo(CardElementClassAttributes attributes, Rarity rarity, float staminaCost)
            : base(attributes, rarity)
        {
            _staminaCost = staminaCost;
        }

        #endregion

        #region Private Methods

        private void SwapPositions()
        {
            if (_yoyo.State is Yoyo.YoyoState.IDLE) return;
            //TODO: animation?
            _playerController.transform.position = _yoyo.transform.position;
            if (_yoyo.State is Yoyo.YoyoState.PRECISION)
                _yoyo.CancelPrecision();
        }

        #endregion
    }
}