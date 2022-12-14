using Player;

namespace Cards.Buffs.ActiveBuffs
{
    public class SwapPositionWithYoyo : IBuff
    {
        private PlayerController _playerController;
        private Yoyo _yoyo;

        public void Apply(PlayerController playerController)
        {
            playerController.DashStartEvent += SwapPositions;
            _playerController = playerController;
            _yoyo = playerController.Yoyo;
        }

        private void SwapPositions()
        {
            if (_yoyo.State is Yoyo.YoyoState.IDLE) return;
            //TODO: animation?
            _playerController.transform.position = _yoyo.transform.position;
        }
    }
}