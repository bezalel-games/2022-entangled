using Player;
using Player.Yoyo;

namespace Cards.Buffs.ActiveBuffs
{
    public class SwapPositionWithYoyo : Buff
    {
        private PlayerController _playerController;
        private Yoyo _yoyo;

        protected override void ApplyBuff(PlayerController playerController)
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