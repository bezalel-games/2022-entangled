using Player;

namespace Cards.Buffs.ActiveBuffs
{
    public class SwapPositionWithYoyo : IBuff
    {
        #region Fields

        private PlayerController _playerController;
        private Yoyo _yoyo;

        #endregion

        #region ICardProperty Implementation

        public string Name => "Dimensional Glitch";

        public string Description =>
            "Instantly teleport to your weapon's position by pressing L1 when you are not holding it";

        public string Rarity => "Normal";

        #endregion

        #region IBuff Implementation

        public void Apply(PlayerController playerController)
        {
            playerController.DashStartEvent += SwapPositions;
            _playerController = playerController;
            _yoyo = playerController.Yoyo;
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