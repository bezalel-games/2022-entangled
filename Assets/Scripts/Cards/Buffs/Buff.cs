using Player;

namespace Cards.Buffs
{
    public abstract class Buff
    {
        private bool _applied = false;
        public void Apply(PlayerController playerController)
        {
            if (_applied) return;
            ApplyBuff(playerController);
            _applied = true;
        } 
        protected abstract void ApplyBuff(PlayerController playerController);
    }
}